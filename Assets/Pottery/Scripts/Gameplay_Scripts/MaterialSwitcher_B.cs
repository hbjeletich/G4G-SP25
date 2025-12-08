using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Pottery
{
    // Switches materials based on user input for colors and decals//
    // Also manages saving, and resetting material selections//

    public class MaterialSwitcher_B : MonoBehaviour
    {
        public Renderer targetRenderer;
        public Material[] materials;

        //Assign MATS in inspector//
        public Material solidRed;
        public Material solidBlue;
        public Material solidYellow;
        public Material solidWhite;

        // COLOR FLAGS //
        private bool isBaseDecal = false;
        private bool isDecalRed = false;
        private bool isDecalWhite = false;
        private bool isDecalBlue = false;
        private bool isDecalYellow = false;

        public Material ClayMaterial;

        // DECAL MATERIALS // 
        public Material redDecalOMaterial;
        public Material redDecalSMaterial;
        public Material redDecalAMaterial;

        public Material blueDecalOMaterial;
        public Material blueDecalSMaterial;
        public Material blueDecalAMaterial;

        public Material yellowDecalOMaterial;
        public Material yellowDecalSMaterial;
        public Material yellowDecalAMaterial;

        public Material whiteDecalOMaterial;
        public Material whiteDecalSMaterial;
        public Material whiteDecalAMaterial;

        public static Material selectedMaterial;
        public static int selectedMaterialIndex = 0;

        // DECAL SELECTED MATERIALS //
        public static Material selectedODecal;
        public static Material selectedSDecal;
        public static Material selectedADecal;

        private enum DecalType { None, O, S, A }
        private DecalType currentDecal = DecalType.None;


        
        void Start()
        {
            if (targetRenderer != null)
            {
                if (selectedMaterial != null)
                {
                    targetRenderer.material = selectedMaterial;
                }
                else
                {
                    selectedMaterial = ClayMaterial; // default
                    targetRenderer.material = ClayMaterial;
                }
            }
        }

        // Color Triggers, if a color is selected, others are deselected//
        public void Redtrigger()
        {
            SetColorFlags(true, false, false, false);
            ApplyCurrentColor();
        }

        public void Whitetrigger()
        {
            SetColorFlags(false, true, false, false);
            ApplyCurrentColor();
        }

        public void Bluetrigger()
        {
            SetColorFlags(false, false, true, false);
            ApplyCurrentColor();
        }

        public void Yellowtrigger()
        {
            SetColorFlags(false, false, false, true);
            ApplyCurrentColor();
        }

        private void SetColorFlags(bool r, bool w, bool b, bool y)
        {
            isDecalRed = r;
            isDecalWhite = w;
            isDecalBlue = b;
            isDecalYellow = y;
        }

        // Set Decal Materials depending on color trigger//
        public void SetRedODecals()
        {
            currentDecal = DecalType.O;
            selectedODecal = redDecalOMaterial;
            ApplyCurrentColor();
        }

        public void SetRedSDecals()
        {
            currentDecal = DecalType.S;
            selectedSDecal = redDecalSMaterial;
            ApplyCurrentColor();
        }

        public void SetRedADecals()
        {
            currentDecal = DecalType.A;
            selectedADecal = redDecalAMaterial;
            ApplyCurrentColor();
        }

        public void SetWhiteODecals()
        {
            currentDecal = DecalType.O;
            selectedODecal = whiteDecalOMaterial;
            ApplyCurrentColor();
        }

        public void SetWhiteSDecals()
        {
            currentDecal = DecalType.S;
            selectedSDecal = whiteDecalSMaterial;
            ApplyCurrentColor();
        }

        public void SetWhiteADecals()
        {
            currentDecal = DecalType.A;
            selectedADecal = whiteDecalAMaterial;
            ApplyCurrentColor();
        }

        public void SetBlueODecals()
        {
            currentDecal = DecalType.O;
            selectedODecal = blueDecalOMaterial;
            ApplyCurrentColor();
        }

        public void SetBlueSDecals()
        {
            currentDecal = DecalType.S;
            selectedSDecal = blueDecalSMaterial;
            ApplyCurrentColor();
        }

        public void SetBlueADecals()
        {
            currentDecal = DecalType.A;
            selectedADecal = blueDecalAMaterial;
            ApplyCurrentColor();
        }

        public void SetYellowODecals()
        {
            currentDecal = DecalType.O;
            selectedODecal = yellowDecalOMaterial;
            ApplyCurrentColor();
        }

        public void SetYellowSDecals()
        {
            currentDecal = DecalType.S;
            selectedSDecal = yellowDecalSMaterial;
            ApplyCurrentColor();
        }

        public void SetYellowADecals()
        {
            currentDecal = DecalType.A;
            selectedADecal = yellowDecalAMaterial;
            ApplyCurrentColor();
        }

        // REMOVE DECAL BUTTON
        public void ClearDecal()
        {
            currentDecal = DecalType.None;
            ApplyCurrentColor();
        }

        // APPLY FINAL MATERIAL
        private void ApplyCurrentColor()
        {
            Material matToApply = solidRed;

            // Solid color selection
            if (isBaseDecal) matToApply = ClayMaterial;
            if (isDecalRed) matToApply = solidRed;
            if (isDecalWhite) matToApply = solidWhite;
            if (isDecalBlue) matToApply = solidBlue;
            if (isDecalYellow) matToApply = solidYellow;

            // Apply decal
            switch (currentDecal)
            {
                case DecalType.O:
                    if (isDecalRed) matToApply = redDecalOMaterial;
                    if (isDecalWhite) matToApply = whiteDecalOMaterial;
                    if (isDecalBlue) matToApply = blueDecalOMaterial;
                    if (isDecalYellow) matToApply = yellowDecalOMaterial;
                    break;

                case DecalType.S:
                    if (isDecalRed) matToApply = redDecalSMaterial;
                    if (isDecalWhite) matToApply = whiteDecalSMaterial;
                    if (isDecalBlue) matToApply = blueDecalSMaterial;
                    if (isDecalYellow) matToApply = yellowDecalSMaterial;
                    break;

                case DecalType.A:
                    if (isDecalRed) matToApply = redDecalAMaterial;
                    if (isDecalWhite) matToApply = whiteDecalAMaterial;
                    if (isDecalBlue) matToApply = blueDecalAMaterial;
                    if (isDecalYellow) matToApply = yellowDecalAMaterial;
                    break;

                case DecalType.None:
                    break;
            }

            selectedMaterial = matToApply;
            targetRenderer.material = matToApply;
        }

        // RESET MATERIAL TO DEFAULT MAT// 
        public void ResetMaterial()
        {
            StartCoroutine(ResetMaterialDelayed());
        }


        private IEnumerator ResetMaterialDelayed()
        {
            // Wait exactly 3 frames//
            //gives time for other processes, etc screenshot capture to complete//
            yield return null;
            yield return null;
            yield return null;
           

            // Clear all color triggers//
            isDecalRed = false;
            isDecalWhite = false;
            isDecalBlue = false;
            isDecalYellow = false;

            // Clear decal selection //
            currentDecal = DecalType.None;

            // Reset selected material //
            selectedMaterial = ClayMaterial;

            if (targetRenderer != null)
                targetRenderer.material = ClayMaterial;
        }
    }
}