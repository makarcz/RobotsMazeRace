using System;
using System.Drawing;
using Microsoft.DirectX;
using Microsoft.DirectX.Direct3D;
using Direct3D=Microsoft.DirectX.Direct3D;
using System.Text;
using System.IO;

//namespace Tew.DirectX.DirectX3D
namespace RobEnvMK
{
	public struct Vertex
	{
		public Vector3 p;
		public Vector3 n;
		public static readonly VertexFormats Format = VertexFormats.Position | VertexFormats.Normal;
	}
	
	public struct LineVertex
	{
		public Vector3 p;
		public static readonly VertexFormats Format = VertexFormats.Position;
	}


	/// <summary>
	/// Summary description for Sphere3D.
	/// </summary>
	public class Sphere3D : IDisposable
	{
		private static long instanceCount = 0;
		private long instanceId = 0;
		
		
		private Mesh mesh = null;
		private Material material;
		
		private Matrix rotate = Matrix.Identity;
		private Matrix translate = Matrix.Identity;
		
		private string name = "";
		private Vector3 position;
		private int numberFaces = 0;
		private int numberVertices = 0;
		private float radius = 0;

		public Sphere3D(Device device, Vector3 position, float radius, int slices, int stacks )
		{
			
			mesh = Mesh.Sphere(device, radius, slices, stacks);
			Position = position;
			
			numberFaces = mesh.NumberFaces;
			numberVertices =  mesh.NumberVertices;
			this.radius = radius;
			
			name = "Sphere3D " + instanceCount.ToString();
			instanceId = instanceCount;
			instanceCount++;
		}
		
		public Microsoft.DirectX.Direct3D.Material Material
		{
			get
			{
				return material;
			}
			set
			{
				material = value;
			}
		}
		
		public int NumberFaces
		{
			get
			{
				return numberFaces;
			}
		}
		
		
		public int NumberVertices
		{
			get
			{
				return numberVertices;
			}
		}
		
		public Mesh SphereMesh
		{
			get
			{
				return mesh;
			}
		}		
		
		public Microsoft.DirectX.Vector3 Position
		{
			get
			{
				return position;
			}
			
			set
			{
				//Reset position to Origin
				//if(position.X !=0 & position.Y != 0 & position.Z !=0)
				if(position.X !=0 || position.Y != 0 || position.Z !=0)
				{
					translate.Translate(-position.X, -position.Y, -position.Z);
					try
					{
						VertexBuffer sourceVB = mesh.VertexBuffer;
						Vertex[] src = (Vertex[])sourceVB.Lock(0, typeof(Vertex), 0, mesh.NumberVertices);
						for (int i = 0; i < src.Length; i++)
						{					
							src[i].p = Vector3.TransformCoordinate(src[i].p, translate);
						}
						sourceVB.Unlock();
						sourceVB.Dispose();
					}
					
					//catch(SampleException ex)
					//{
					//	Console.WriteLine(ex);
					//}
					catch(Exception ex)
					{
						Console.WriteLine(ex);
					}
				}
				
				//Move to new position
				{
					translate.Translate(value.X, value.Y, value.Z);
					try
					{
						VertexBuffer sourceVB = mesh.VertexBuffer;
						Vertex[] src = (Vertex[])sourceVB.Lock(0, typeof(Vertex), 0, mesh.NumberVertices);
						for (int i = 0; i < src.Length; i++)
						{					
							src[i].p = Vector3.TransformCoordinate(src[i].p, translate);
						}
						sourceVB.Unlock();
						sourceVB.Dispose();
					}
						
					//catch(SampleException ex)
					//{
					//	Console.WriteLine(ex);
					//}
					catch(Exception ex)
					{
						Console.WriteLine(ex);
					}
				}	
				
				position = value;
			}
		}
		
		
		public float Radius 
		{
			get
			{
				return radius;
			}
		}
		
		
		public void DrawSubset(int attributeId)
		{
			
			mesh.DrawSubset(attributeId);
		}
		
		
		public void Dispose()
		{
			 ClearMeshData();
		}
		
		private void ClearMeshData()
		{
			numberFaces = 0;
			numberVertices = 0;
			mesh.Dispose();
			mesh = null;
		}
		
		public Microsoft.DirectX.Matrix TranslationMatrix
		{
			get
			{
				return translate;
			}
			set
			{
				translate = value;
			}
		}
		
		
		public long GetInstanceId()
		{
			return instanceId;
		}
	
	
		public string Name
		{
			get
			{
				if (name != "")
				{
					return name;	
				}
				else
				{
					return this.GetType() + "  Instance ID:" + instanceId;
				}
			}
			set
			{
				name = value;
			}
			
		}
		
		
	}//~Class: Sphere3D
}//~namespace

