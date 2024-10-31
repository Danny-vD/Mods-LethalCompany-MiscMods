// credit: https://github.com/dsapandora/UnityObjExporter

using System.Text;
using UnityEngine;

namespace ExtraInformation.AssetRippers
{
	public static class MeshToObj
	{
		public static string GetObj(GameObject gameObject, out string meshName, bool useCurrentTransform = false)
		{
			meshName = string.Empty;
			Transform transform = gameObject.transform;
			MeshFilter meshFilter = gameObject.GetComponent<MeshFilter>();

			if (ReferenceEquals(meshFilter, null))
			{
				return "####Error####";
			}
			
			Quaternion rotation = transform.localRotation;
			Mesh mesh = meshFilter.sharedMesh;

			if (!mesh)
			{
				return "####Error####";
			}

			if (!mesh.isReadable)
			{
				mesh = MeshUtils.MakeReadableMeshCopy(mesh);
			}
			
			meshName = mesh.name;

			if (meshName == string.Empty)
			{
				meshName = NameUtils.RemoveCloneFromString(gameObject.name);
			}

			Material[] mats = meshFilter.GetComponent<Renderer>().sharedMaterials;

			StringBuilder sb = new StringBuilder();

			foreach (Vector3 vv in mesh.vertices)
			{
				Vector3 v = useCurrentTransform ? transform.TransformPoint(vv) : vv;
				sb.Append($"v {v.x} {v.y} {v.z}\n");
				
				//sb.Append($"v {v.x} {v.y} {-v.z}\n");
			}

			sb.Append("\n");

			foreach (Vector3 nn in mesh.normals)
			{
				Vector3 v = useCurrentTransform ? rotation * nn : nn;
				sb.Append($"vn {v.x} {v.y} {v.z}\n");
				
				//sb.Append($"vn {-v.x} {-v.y} {v.z}\n");
			}

			sb.Append("\n");

			foreach (Vector2 v in mesh.uv)
			{
				sb.Append($"vt {v.x} {v.y}\n");
			}

			for (int material = 0; material < mesh.subMeshCount; material++)
			{
				sb.Append("\n");
				sb.Append("usemtl ").Append(mats[material].name).Append("\n");
				sb.Append("usemap ").Append(mats[material].name).Append("\n");

				int[] triangles = mesh.GetTriangles(material);

				for (int i = 0; i < triangles.Length; i += 3)
				{
					sb.Append(string.Format("f {0}/{0}/{0} {1}/{1}/{1} {2}/{2}/{2}\n",
						triangles[i] + 1, triangles[i + 1] + 1, triangles[i + 2] + 1));
				}
			}
			
			return sb.ToString();
		}
	}
}