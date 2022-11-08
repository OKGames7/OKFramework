using UnityEngine;
using System.Collections;
using System.IO;
using UnityEditor;
using System.Xml.Serialization;
using NPOI.HSSF.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.SS.UserModel;

public class platform_items_importer : AssetPostprocessor {
	private static readonly string filePath = "Assets/01_PJ/MasterData/OriginData/platform_items.xlsx";
	private static readonly string exportPath = "Assets/Addressable/Asset/Masterdata/platform_items.asset";
	private static readonly string[] sheetNames = { "platform_item", };

	static void OnPostprocessAllAssets (string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromAssetPaths)
	{
		foreach (string asset in importedAssets) {
			if (!filePath.Equals (asset))
				continue;

			Entity_platform_item data = (Entity_platform_item)AssetDatabase.LoadAssetAtPath (exportPath, typeof(Entity_platform_item));
			if (data == null) {
				data = ScriptableObject.CreateInstance<Entity_platform_item> ();
				AssetDatabase.CreateAsset ((ScriptableObject)data, exportPath);
				data.hideFlags = HideFlags.NotEditable;
			}

			data.sheets.Clear ();
			using (FileStream stream = File.Open (filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite)) {
				IWorkbook book = null;
				if (Path.GetExtension (filePath) == ".xls") {
					book = new HSSFWorkbook(stream);
				} else {
					book = new XSSFWorkbook(stream);
				}

				foreach(string sheetName in sheetNames) {
					ISheet sheet = book.GetSheet(sheetName);
					if( sheet == null ) {
						Debug.LogError("[QuestData] sheet not found:" + sheetName);
						continue;
					}

					Entity_platform_item.Sheet s = new Entity_platform_item.Sheet ();
					s.name = sheetName;

					for (int i=1; i<= sheet.LastRowNum; i++) {
						IRow row = sheet.GetRow (i);
						ICell cell = null;

						Entity_platform_item.Param p = new Entity_platform_item.Param ();
						
					cell = row.GetCell(0); p.ID = (int)(cell == null ? 0 : cell.NumericCellValue);
					cell = row.GetCell(1); p.productID = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(2); p.productType = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(3); p.platform = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(4); p.titleKey = (cell == null ? "" : cell.StringCellValue);
					cell = row.GetCell(5); p.discriptionKey = (cell == null ? "" : cell.StringCellValue);
						s.list.Add (p);
					}
					data.sheets.Add(s);
				}
			}

			ScriptableObject obj = AssetDatabase.LoadAssetAtPath (exportPath, typeof(ScriptableObject)) as ScriptableObject;
			EditorUtility.SetDirty (obj);
		}
	}
}
