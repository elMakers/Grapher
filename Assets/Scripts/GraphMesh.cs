using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class GraphMesh : MonoBehaviour
{
	public float ScaleX = 0.001f;
	public float ScaleY = 0.00005f;
	public float ScaleZ = 0.01f;
	public int VertexLimit = 100000;
	public String DataFile = "data.dat";

	void AddRow(int x, List<float> row, List<Vector3> vertices, List<int> triangles)
	{
		if (row.Count != 100) Debug.Log("WHAT? " + row.Count);
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
		row.Clear();
	}
	
	// Use this for initialization
	void Start ()
	{
		List<Vector3> vertices = new List<Vector3>();
		List<int> triangles = new List<int>();
			
		// Import gnuplot file
		int x = 0;
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
					AddRow(x, currentRow, vertices, triangles);
					x++;
				}
				if (vertices.Count > VertexLimit) break;
				continue;
			}

			String[] pieces = line.Split(' ');
			currentRow.Add((float)Double.Parse(pieces[2]));
		}

		//AddRow(x, currentRow, vertices, triangles);
		reader.Close();
		Debug.Log("Loaded " + vertices.Count + " verts x " + triangles.Count + " triangles");
		
		Vector2[] uvs = new Vector2[vertices.Count];
		for (var i = 0; i < uvs.Length; i++) //Give UV coords X,Z world coords
			uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
		
		Mesh mesh = new Mesh();
		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uvs;

		GetComponent<MeshFilter>().mesh = mesh;
	}
}
