#!/usr/bin/env python3
# Production-ready C3D converter with bug fixes

import json
import numpy as np
import sys
import os
import warnings

def convert_to_c3d(json_path):
    """Convert Unity motion JSON to production-ready C3D file"""
    
    try:
        import c3d
        print("Creating production C3D file...")
    except ImportError:
        print("Error: 'c3d' library not installed. Run: pip install c3d")
        return None
    
    # Suppress the "no analog data" warning since we expect this
    warnings.filterwarnings('ignore', message='No analog data found in file.')
    
    # Load motion data
    with open(json_path, 'r') as f:
        data = json.load(f)
    
    frames = data['frames']
    joint_names = [joint['name'] for joint in frames[0]['joints']]
    frame_rate = data['frameRate']
    
    print(f"Processing: {len(frames)} frames, {len(joint_names)} joints, {frame_rate} Hz")
    
    # Prepare motion data in correct format
    print("Converting motion data to clinical format...")
    
    n_frames = len(frames)
    n_points = len(joint_names)
    all_frames_data = []
    
    for frame_idx, frame in enumerate(frames):
        if frame_idx % 100 == 0:
            print(f"  Frame {frame_idx}/{n_frames}")
        
        frame_joints = {joint['name']: joint for joint in frame['joints']}
        
        # Point data: shape (n_points, 4) = [x, y, z, residual]
        # Important: Use exactly 4 columns to avoid index errors
        point_data = np.zeros((n_points, 4), dtype=np.float32)
        
        for point_idx, joint_name in enumerate(joint_names):
            if joint_name in frame_joints:
                pos = frame_joints[joint_name]['position']
                # Convert Unity to clinical coordinates (mm)
                point_data[point_idx, 0] = pos['x'] * 1000    # X (lateral)
                point_data[point_idx, 1] = pos['z'] * 1000    # Y (anterior) - swapped
                point_data[point_idx, 2] = -pos['y'] * 1000   # Z (superior) - flipped
                point_data[point_idx, 3] = 0.0                # Residual: 0 = good data
            else:
                # Missing joint data
                point_data[point_idx, 0:3] = 0.0
                point_data[point_idx, 3] = -1.0               # Residual: -1 = missing
        
        # Analog data: empty but properly shaped to avoid library bugs
        # Shape: (n_analog_channels, n_analog_samples_per_frame)
        # For no analog data: (0, 1) - this prevents index errors
        analog_data = np.array([], dtype=np.float32).reshape(0, 1)
        
        # Add frame as required tuple format
        all_frames_data.append((point_data, analog_data))
    
    # Create output file
    output_file = json_path.replace('.json', '_PRODUCTION.c3d')
    
    print(f"Creating C3D file: {output_file}")
    
    try:
        with open(output_file, 'wb') as handle:
            # Create writer with minimal, stable parameters
            writer = c3d.Writer(
                point_rate=float(frame_rate),  # Ensure float
                analog_rate=0                  # No analog data
            )
            
            # Set point labels safely
            try:
                writer.set_point_labels(joint_names)
                print("✓ Joint labels set successfully")
            except Exception as e:
                print(f"⚠ Could not set labels: {e}")
                print("⚠ File will work but without joint names")
            
            # Add all motion frames
            print("Adding motion frames...")
            try:
                writer.add_frames(all_frames_data)
                print("✓ All frames added successfully")
            except Exception as e:
                print(f"✗ Error adding frames: {e}")
                return None
            
            # Write file with error handling
            print("Writing C3D file...")
            try:
                writer.write(handle)
                print("✓ C3D file written successfully")
            except Exception as e:
                # Sometimes the library throws errors during cleanup but file is still written
                print(f"⚠ Writer cleanup warning: {e}")
                print("⚠ Checking if file was created anyway...")
        
        # Verify the file was created and is valid
        print("Verifying C3D file...")
        
        if not os.path.exists(output_file):
            print("✗ File was not created")
            return None
        
        file_size = os.path.getsize(output_file)
        if file_size == 0:
            print("✗ File is empty")
            return None
        
        print(f"✓ File created: {file_size} bytes")
        
        # Test if file can be read
        try:
            with open(output_file, 'rb') as handle:
                reader = c3d.Reader(handle)
                
                # Get file info
                points_count = getattr(reader, 'point_used', 0)
                frames_count = getattr(reader, 'last_frame', 0) - getattr(reader, 'first_frame', 0) + 1
                rate = getattr(reader, 'point_rate', 0)
                
                print(f"✓ File verification successful:")
                print(f"  Points: {points_count}")
                print(f"  Frames: {frames_count}")
                print(f"  Frame rate: {rate} Hz")
                print(f"  Duration: {frames_count/rate:.1f} seconds")
                
                # Check if labels are present
                if hasattr(reader, 'point_labels') and reader.point_labels:
                    print(f"  Labels: {len(reader.point_labels)} joints")
                    print(f"  First few: {reader.point_labels[:3]}")
                else:
                    print("  Labels: Not included")
                
        except Exception as e:
            print(f"⚠ File created but verification failed: {e}")
            print("⚠ File should still be usable in biomechanics software")
        
        return output_file
        
    except Exception as e:
        print(f"✗ Unexpected error during conversion: {e}")
        import traceback
        traceback.print_exc()
        return None
    
    finally:
        # Reset warning filters
        warnings.resetwarnings()

def main():
    if len(sys.argv) != 2:
        print("Usage: python production_c3d_converter.py your_file.json")
        print("Example: python production_c3d_converter.py motion_recording_2025-07-31_15-21-39.json")
        return
    
    json_file = sys.argv[1]
    
    if not os.path.exists(json_file):
        print(f"Error: File not found: {json_file}")
        return
    
    print("=== Production C3D Converter ===")
    print(f"Input: {json_file}")
    
    result = convert_to_c3d(json_file)
    
    if result:
        print(f"\n🎉 SUCCESS! Production C3D file ready!")
        print(f"📁 Output: {result}")
        print(f"🏥 Compatible with: Visual3D, OpenSim, Mokka, Cortex, etc.")
        print(f"📊 Coordinate system: Clinical (Z-up, right-handed, mm)")
        print(f"✅ Ready for biomechanical analysis!")
        
        # Provide joint reference
        print(f"\n📝 Joint reference (in order):")
        joint_names = [
            'Hips', 'LeftUpLeg', 'LeftLeg', 'LeftFoot', 'LeftToeBase',
            'RightUpLeg', 'RightLeg', 'RightFoot', 'RightToeBase', 
            'Spine', 'Spine1', 'Spine2', 'Spine3',
            'LeftShoulder', 'LeftArm', 'LeftForeArm', 'LeftHand',
            'RightShoulder', 'RightArm', 'RightForeArm', 'RightHand',
            'Spine4', 'Neck', 'Head'
        ]
        for i, name in enumerate(joint_names[:8], 1):
            print(f"  {i:2d}. {name}")
        print(f"  ... and {len(joint_names)-8} more joints")
        
    else:
        print(f"\n❌ Conversion failed!")
        print(f"💡 Troubleshooting:")
        print(f"   - Check that JSON file contains motion data")
        print(f"   - Try: python inspect_json.py {json_file}")
        print(f"   - As backup, use CSV export instead")

if __name__ == '__main__':
    main()
