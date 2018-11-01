using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class GraphMesh : MonoBehaviour
{
	public int SkipX = 0;
	public float ScaleX = 0.001f;
	public float ScaleY = 0.00005f;
	public float ScaleZ = 0.01f;
	public float LabelScale = 0.05f;
	public int ZLabelFrequency = 10;
	public int VertexLimit = 0;
	public float OutlierY = 0.0f;
	public String DataFile = "data.dat";

	void AddRow(int x, List<float> row, List<Vector3> vertices, List<int> triangles)
	{
		for (int z = 0; z < row.Count; z++)
		{
			//Add each new vertex in the plane
			vertices.Add(new Vector3(ScaleX * x, row[z] * ScaleY, ScaleZ * z));

			//Skip if a new square on the plane hasn't been formed
			if (x == 0 || z == 0) continue;

			//Adds the index of the three vertices in order to make up each of the two tris
			triangles.Add(row.Count * x + z); //Top right
			triangles.Add(row.Count * x + z - 1); //Bottom right
			triangles.Add(row.Count * (x - 1) + z - 1); //Bottom left - First triangle
			triangles.Add(row.Count * (x - 1) + z - 1); //Bottom left 
			triangles.Add(row.Count * (x - 1) + z); //Top left
			triangles.Add(row.Count * x + z); //Top right - Second triangle
		}
	}
	
	// Use this for initialization
	void Start ()
	{
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
		List<double> zLabels = new List<double>();

		if (SkipX > 0)
		{
			ScaleX *= SkipX;
		}
			
		// Import gnuplot file
		int x = 0;
		int rowNumber = 0;
		Debug.Log("Loading: " + Application.persistentDataPath + '/' + DataFile);
		StreamReader reader = new StreamReader(Application.persistentDataPath + '/' + DataFile);
		List<float> currentRow = new List<float>();
		while (!reader.EndOfStream)
		{
			String line = reader.ReadLine();
			if (line.StartsWith("#")) continue;
			line = line.Trim();
			if (line.Length == 0)
			{
				if (currentRow.Count > 0)
				{
					if (SkipX == 0 || rowNumber % SkipX == 0)
					{
						AddRow(x, currentRow, vertices, triangles);
						x++;
					}
					currentRow.Clear();

					rowNumber++;
				}
				if (VertexLimit > 0 && vertices.Count > VertexLimit) break;
				continue;
			}

			String[] pieces = line.Split(' ');
			float value = (float) Double.Parse(pieces[2]);
			if (x == 0)
			{
				float label = (float)Double.Parse(pieces[1]);
				zLabels.Add(label);
			}
			if (OutlierY > 0 && value > OutlierY) value = 0;
			currentRow.Add(value);
		}

		if (currentRow.Count > 0)
		{
			AddRow(x, currentRow, vertices, triangles);
		}

		reader.Close();
		Debug.Log("Loaded " + vertices.Count + " verts x " + triangles.Count + " triangles");

		Vector3[] normals = new Vector3[vertices.Count];
		for (var i = 0; i < normals.Length; i++) //Give UV coords X,Z world coords
			normals[i] = Vector3.up;
		
		Vector2[] uvs = new Vector2[vertices.Count];
		for (var i = 0; i < uvs.Length; i++) //Give UV coords X,Z world coords
			uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
		
		Mesh mesh = new Mesh();
		mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs;
		mesh.normals = normals;
		// MeshUtility.Optimize(mesh);

		transform.position = new Vector3(-ScaleX * x / 2, 0, -ScaleZ * currentRow.Count / 2);

		GetComponent<MeshFilter>().mesh = mesh;

		Debug.Log("Creating " + zLabels.Count + " Price labels");
		// Add labels
		for (var labelZ = 0; labelZ < zLabels.Count; labelZ += ZLabelFrequency)
		{
			var testObject = new GameObject();
			TextMesh text = testObject.AddComponent<TextMesh>();
			text.text = zLabels[labelZ].ToString("C");
			text.anchor = TextAnchor.MiddleRight;
			testObject.transform.parent = transform;
			testObject.transform.position = new Vector3(-ScaleX * x / 2, 0, labelZ * ScaleZ);
			testObject.transform.localScale = Vector3.one * LabelScale;
		}
	}
}
