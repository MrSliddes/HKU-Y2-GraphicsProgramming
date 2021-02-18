using System;
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

		Model sphere;
		Texture2D day, night, clouds, moon;

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

			sphere = Content.Load<Model>("uv_sphere");
			day = Content.Load<Texture2D>("day");
			night = Content.Load<Texture2D>("night");
			clouds = Content.Load<Texture2D>("clouds");
			moon = Content.Load<Texture2D>("2k_moon");

			foreach(ModelMesh mesh in sphere.Meshes)
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

			Vector3 cameraPos = -Vector3.Forward * 10;// + Vector3.Up * 5;// + Vector3.Right * 5;
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

			myEffect.Parameters["LightPosition"].SetValue(LightPosition);
			myEffect.Parameters["CameraPosition"].SetValue(cameraPos);

			myEffect.Parameters["Time"].SetValue(time);

			myEffect.CurrentTechnique.Passes[0].Apply();

			device.Clear(Color.Black);

			// Earth
			myEffect.CurrentTechnique = myEffect.Techniques["Earth"];
			RenderModel(sphere, Matrix.CreateScale(0.01f) * Matrix.CreateRotationZ(time) * Matrix.CreateRotationY(MathF.PI / 180 * 23) * World);

			// Moon
			myEffect.CurrentTechnique = myEffect.Techniques["Moon"];
			RenderModel(sphere, Matrix.CreateTranslation(Vector3.Down * 8) * Matrix.CreateScale(0.0033f) * Matrix.CreateRotationZ(time - time * 0.3333333f) * World);
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