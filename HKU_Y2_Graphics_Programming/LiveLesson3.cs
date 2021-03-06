﻿using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace HKU_Y2_Graphics_Programming
{
	class LiveLesson3 : Lesson
	{		
		private Effect myEffect;
		Vector3 LightPosition = Vector3.Right * 2 + Vector3.Up * 2 + Vector3.Backward * 2;

		Model sphere, cube;
		Texture2D day, night, clouds, moon;
		TextureCube sky;

		float yaw, pitch;
		int prevX, prevY;

		public override void Update(GameTime gameTime)
        {
			MouseState mState = Mouse.GetState();

			if(mState.LeftButton == ButtonState.Pressed)
            {
				// Update yaw and pitch
				yaw += (mState.X - prevX) * 0.01f;
				pitch += (mState.Y - prevY) * 0.01f;

				pitch = MathF.Min(MathF.Max(pitch, -MathF.PI * 0.49f), MathF.PI * 0.49f);
            }

			prevX = mState.X;
			prevY = mState.Y;
        }

		public override void LoadContent(ContentManager Content, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			//effect = new BasicEffect(graphics.GraphicsDevice);
			myEffect = Content.Load<Effect>("LiveLesson3");
			//crateTexture = Content.Load<Texture2D>("texture_crate_0");
			//crateNormal = Content.Load<Texture2D>("texture_crate_0_normalmap");

			day = Content.Load<Texture2D>("day");
			night = Content.Load<Texture2D>("night");//jerry
			clouds = Content.Load<Texture2D>("clouds");
			moon = Content.Load<Texture2D>("2k_moon");//cheese
			sky = Content.Load<TextureCube>("sky_cube");

			sphere = Content.Load<Model>("uv_sphere");

			foreach(ModelMesh mesh in sphere.Meshes)
            {
                foreach(ModelMeshPart meshPart in mesh.MeshParts)
                {
					meshPart.Effect = myEffect;
                }
            }

			cube = Content.Load<Model>("cube");

			foreach(ModelMesh mesh in cube.Meshes)
			{
				foreach(ModelMeshPart meshPart in mesh.MeshParts)
				{
					meshPart.Effect = myEffect;
				}
			}
		}

		public override void Draw(GameTime gameTime, GraphicsDeviceManager graphics, SpriteBatch spriteBatch)
		{
			GraphicsDevice device = graphics.GraphicsDevice;

			float time = (float)gameTime.TotalGameTime.TotalSeconds;
			//LightPosition = new Vector3(MathF.Cos(time), 0, MathF.Sin(time)) * 200;
			LightPosition = Vector3.Left * 200;

			Vector3 cameraPos = -Vector3.Forward * 40;// + Vector3.Up * 5;// + Vector3.Right * 5;
			cameraPos = Vector3.Transform(cameraPos, Quaternion.CreateFromYawPitchRoll(yaw, pitch, 0));

			Matrix World = Matrix.CreateWorld(Vector3.Zero, Vector3.Forward, Vector3.Up);
			Matrix View = Matrix.CreateLookAt(cameraPos, Vector3.Zero, Vector3.Up);

			myEffect.Parameters["World"].SetValue(World);
			myEffect.Parameters["View"].SetValue(View);
			myEffect.Parameters["Projection"].SetValue(Matrix.CreatePerspectiveFieldOfView((MathF.PI / 180f) * 25f, device.Viewport.AspectRatio, 0.001f, 1000f));

			myEffect.Parameters["DayTex"].SetValue(day);
			myEffect.Parameters["NightTex"].SetValue(night);
			myEffect.Parameters["CloudsTex"].SetValue(clouds);
			myEffect.Parameters["MoonTex"].SetValue(moon);
			myEffect.Parameters["SkyTex"].SetValue(sky);

			myEffect.Parameters["LightPosition"].SetValue(LightPosition);
			myEffect.Parameters["CameraPosition"].SetValue(cameraPos);

			myEffect.Parameters["Time"].SetValue(time);

			myEffect.CurrentTechnique.Passes[0].Apply();

			device.Clear(Color.Black);

			float slowedTime = time * 0.8f;

			// Sky
			device.RasterizerState = RasterizerState.CullNone;
			myEffect.CurrentTechnique = myEffect.Techniques["Sky"];
			device.DepthStencilState = DepthStencilState.None;
			RenderModel(cube, Matrix.CreateTranslation(cameraPos));

			device.DepthStencilState = DepthStencilState.Default;
			device.RasterizerState = RasterizerState.CullCounterClockwise;

			Random rand = new Random(123);

            for(int x = -5; x < 5; x++)
            {
                for(int z = -5; z < 5; z++)
                {					
                    switch(rand.Next(0, 2))
                    {
						case 0:
							RenderEarthWithDancingMoons(new Vector3(x, z, 0) * 0.1f, World, slowedTime, 15);
							break;
                        default:
							RenderEarthWithMoon(new Vector3(x, z, 0) * 0.1f, World, slowedTime);
                            break;
                    }
                }
            }

			// Earth
			//myEffect.CurrentTechnique = myEffect.Techniques["Earth"];
			//RenderModel(sphere, Matrix.CreateScale(0.01f) * Matrix.CreateRotationZ(slowedTime) * Matrix.CreateRotationY(MathF.PI / 180 * 23) * World);

			// Moon
			//myEffect.CurrentTechnique = myEffect.Techniques["Moon"];
			//RenderModel(sphere, Matrix.CreateTranslation(Vector3.Down * 8) * Matrix.CreateScale(0.0033f) * Matrix.CreateRotationZ(slowedTime - slowedTime * 0.3333333f) * World);			
		}

		void RenderEarthWithMoon(Vector3 pos, Matrix World, float slowedTime)
        {
			// Earth
			myEffect.CurrentTechnique = myEffect.Techniques["Earth"];
			RenderModel(sphere, Matrix.CreateScale(0.01f) * Matrix.CreateRotationZ(slowedTime) * Matrix.CreateRotationY(MathF.PI / 180 * 23) * World * Matrix.CreateTranslation(pos));
			// Moon
			myEffect.CurrentTechnique = myEffect.Techniques["Moon"];
			RenderModel(sphere, Matrix.CreateTranslation(Vector3.Down * 8) * Matrix.CreateScale(0.0033f) * Matrix.CreateRotationZ(slowedTime - slowedTime * 0.3333333f) * World * Matrix.CreateTranslation(pos));
		}

		void RenderEarthWithDancingMoons(Vector3 pos, Matrix World, float slowedTime, int moonAmount)
        {
			// Earth
			myEffect.CurrentTechnique = myEffect.Techniques["Earth"];
			RenderModel(sphere, Matrix.CreateScale(0.01f) * Matrix.CreateRotationZ(slowedTime) * Matrix.CreateRotationY(MathF.PI / 180 * 23) * World * Matrix.CreateTranslation(pos));
			// Moon
			myEffect.CurrentTechnique = myEffect.Techniques["Moon"];

			for(int i = 0; i < moonAmount; i++)
			{
				RenderModel(sphere,
					Matrix.CreateTranslation(Vector3.Down * 12)
					* Matrix.CreateTranslation(Vector3.Forward * MathF.Sin(slowedTime + i))
					* Matrix.CreateRotationX(MathF.Cos(slowedTime))
					* Matrix.CreateRotationZ(((360f / moonAmount) * i) * (float)Math.PI / 180f)
					* Matrix.CreateScale(0.0033f)
					* Matrix.CreateRotationZ(slowedTime - slowedTime * 0.3333333f)
					* World * Matrix.CreateTranslation(pos));
			}
		}


		void RenderModel(Model m, Matrix parentMatrix)
        {
			Matrix[] transforms = new Matrix[m.Bones.Count];
			m.CopyAbsoluteBoneTransformsTo(transforms);

			myEffect.CurrentTechnique.Passes[0].Apply();

			foreach(ModelMesh mesh in m.Meshes)
            {
				myEffect.Parameters["World"].SetValue(parentMatrix * transforms[mesh.ParentBone.Index]);

				mesh.Draw();
            }
        }
	}
}