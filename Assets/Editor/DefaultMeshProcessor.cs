using UnityEditor;

public class DefaultMeshProcessor : AssetPostprocessor {
    public void OnPreprocessModel() {
		ModelImporter modelImporter = (ModelImporter) base.assetImporter;
		modelImporter.importMaterials = false;
		modelImporter.importAnimation = false;
    }
}