using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace HKU_Y2_Graphics_Programming
{
	class LiveLesson1 : Lesson
	{
		// Vertex with position and color
		// The direction on which you write vertex decides the backface (rightaround = facing you, leftaround = facing away from you)
		// 0, 0				1, 0
		// 0, -1			1, -1
		VertexPositionColor[] vertices = {
			//+z (front) square
			new VertexPositionColor( new Vector3( -0.5f, 0.5f, 0.5f ), Color.Red ),
			new VertexPositionColor( new Vector3( 0.5f, -0.5f, 0.5f ), Color.Green ),
			new VertexPositionColor( new Vector3( -0.5f, -0.5f, 0.5f ), Color.Blue ),
			new VertexPositionColor( new Vector3( 0.5f, 0.5f, 0.5f ), Color.Yellow ),
			
			//-z (back)
			new VertexPositionColor( new Vector3( -0.5f, 0.5f, -0.5f ), Color.Red ),
			new VertexPositionColor( new Vector3( 0.5f, -0.5f, -0.5f ), Color.Green ),
			new VertexPositionColor( new Vector3( -0.5f, -0.5f, -0.5f ), Color.Blue ),
			new VertexPositionColor( new Vector3( 0.5f, 0.5f, -0.5f ), Color.Yellow ),

			// -x left
			new VertexPositionColor( new Vector3( -0.5f, 0.5f, 0.5f ), Color.Red ),
			new VertexPositionColor( new Vector3( -0.5f, -0.5f, 0.5f ), Color.Green ),
			new VertexPositionColor( new Vector3( -0.5f, -0.5f, -0.5f ), Color.Blue ),
			new VertexPositionColor( new Vector3( -0.5f, 0.5f, -0.5f ), Color.Yellow ),

			// +x right
			new VertexPositionColor( new Vector3( 0.5f, 0.5f, 0.5f ), Color.Red ),
			new VertexPositionColor( new Vector3( 0.5f, 0.5f, -0.5f ), Color.Green ),
			new VertexPositionColor( new Vector3( 0.5f, -0.5f, 0.5f ), Color.Blue ),
			new VertexPositionColor( new Vector3( 0.5f, -0.5f, -0.5f ), Color.Yellow ),

			// -y bottom
			new VertexPositionColor( new Vector3( -0.5f, -0.5f, 0.5f ), Color.Red ),
			new VertexPositionColor( new Vector3( -0.5f, -0.5f, -0.5f ), Color.Green ),
			new VertexPositionColor( new Vector3( 0.5f, -0.5f, 0.5f ), Color.Blue ),
			new VertexPositionColor( new Vector3( 0.5f, -0.5f, -0.5f ), Color.Yellow ),

			// +y top
			new VertexPositionColor( new Vector3( -0.5f, 0.5f, 0.5f ), Color.Red ),
			new VertexPositionColor( new Vector3( -0.5f, 0.5f, -0.5f ), Color.Green ),
			new VertexPositionColor( new Vector3( 0.5f, 0.5f, 0.5f ), Color.Blue ),
			new VertexPositionColor( new Vector3( 0.5f, 0.5f, -0.5f ), Color.Yellow )
		};

		// Index from the vertices array, (about what vertex are you talking?)
		int[] indices = {
			//FRONT
			//triangle 1
			0, 1, 2,
			//triangle 2
			0, 3, 1,

			//BACK
			//triangle 1
			4, 6, 5,
			//triangle 2
			4, 5, 7,

			// LEFT
			// triangle 1
			8, 9, 10,
			// triangle 2
			10, 11, 8,

			// RIGHT
			// triangle 1
			12, 13, 14,
			// triangle 2
			13, 15, 14,

			// BOTTOM
			// triangle 1
			18, 17, 16,
			// triangle 2
			18, 19, 17,

			// TOP
			// triangle 1
			20, 21, 22,
			// triangle 2
			21, 23, 22
		};


		// Create shader (effect)
		BasicEffect effect;
		public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			effect = new BasicEffect(graphics.GraphicsDevice);
		}

		// Go's to gpu
		public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			// Grab graphics device
			GraphicsDevice device = graphics.GraphicsDevice;
			// Clear screen
			device.Clear(Color.Black);

			// View
			effect.World = Matrix.Identity * Matrix.CreateRotationY((float)gameTime.TotalGameTime.TotalSeconds) * Matrix.CreateRotationX((float)gameTime.TotalGameTime.TotalSeconds);
			effect.View = Matrix.CreateLookAt(-Vector3.Forward * 5, Vector3.Zero, Vector3.Up);
			effect.Projection = Matrix.CreatePerspectiveFieldOfView((MathF.PI / 180f) * 65f, device.Viewport.AspectRatio, 0.1f, 100f);

			// Draw vertices
			effect.VertexColorEnabled = true;
			foreach(EffectPass pass in effect.CurrentTechnique.Passes) // Shader stuff
			{
				pass.Apply();
				device.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, indices.Length / 3);
			}
		}
	}
}