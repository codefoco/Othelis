using System;
using System.Collections.Generic;

#if ANDROID
using Android.App;
#endif

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Storage;

using Microsoft.Xna.Framework.GamerServices;

namespace Othelis
{
	/// <summary>
	/// This is the main type for your game
	/// </summary>
	public class Game1 : Microsoft.Xna.Framework.Game
	{
		GraphicsDeviceManager graphics;
		SpriteBatch spriteBatch;		
		Texture2D texture, ball;
		SpriteFont font;
		float size, rotation;
		float clippingSize = 0.0f;
		Color alphaColor = Color.White;

		PrimitiveBatch primitiveBatch;
		OthelisEngine e = new OthelisEngine();
		Tabuleiro board;
		Tabuleiro temp_board;
		bool wasPressed = false;
		int m_player = 1;
		int xBoard = -1;
		int yBoard = -1;

        public Game1 ()  
		{
			graphics = new GraphicsDeviceManager (this);
			
			Content.RootDirectory = "Content";
			IsMouseVisible = true;

			graphics.SupportedOrientations = DisplayOrientation.Portrait | DisplayOrientation.LandscapeLeft | DisplayOrientation.LandscapeRight | DisplayOrientation.PortraitUpsideDown;			
		}
		
		/// <summary>
		/// Allows the game to perform any initialization it needs to before starting to run.
		/// This is where it can query for any required services and load any non-graphic
		/// related content.  Calling base.Initialize will enumerate through any components
		/// and initialize them as well.
		/// </summary>
		protected override void Initialize ()
		{
			// TODO: Add your initialization logic here
			primitiveBatch = new PrimitiveBatch(graphics.GraphicsDevice);
			e.AlocaTabuleiros (out temp_board);
			board = new Tabuleiro();

			e.IniciaTabuleiro (board);

			base.Initialize ();
		}

		/// <summary>
		/// LoadContent will be called once per game and is the place to load
		/// all of your content.
		/// </summary>
		protected override void LoadContent ()
		{

		}

		/// <summary>
		/// Allows the game to run logic such as updating the world,
		/// checking for collisions, gathering input, and playing audio.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Update (GameTime gameTime)
		{
			if (GamePad.GetState (PlayerIndex.One).Buttons.Back == ButtonState.Pressed) {
				Exit ();
			}

			int h = graphics.GraphicsDevice.Viewport.Height;
				int w = graphics.GraphicsDevice.Viewport.Width;
				int min = Math.Min(h, w);

				int x = Mouse.GetState ().X;
				int y = Mouse.GetState ().Y;
				int margin = min/10;
				min = (min - margin)/8;
				xBoard = (x - margin/2)/min;
				yBoard = (y - margin/2)/min;

			if (Mouse.GetState ().LeftButton == ButtonState.Pressed)
				wasPressed = true;
			else if (wasPressed && m_player == 1) {

				if (e.PodeMoverPedra (xBoard + yBoard*8, board, m_player)) {
					e.MovePedra (xBoard + yBoard*8, board, ref m_player);
				}
				wasPressed = false;

			}

			if (m_player == 2) {

				int	pos = -1, n1, n2, ntot,	nply = 6;
				int ret;
				int pontos = 0;

			e.ContaTotalPedras(board,out n1,out n2);/* Conta o total de peÁas no tabuleiro */

			ntot = n1 + n2 + nply;

		
			e.CopiaTabuleiro (temp_board,board);		

			/* Deixa o computador,verificar melhor jogada */

			ret = e.EncontreMelhorPosicao (temp_board, nply, m_player, ref pontos, ref pos,0, -OthelisEngine.OTH_INFINITO, OthelisEngine.OTH_INFINITO);

			if(ret != -1 && pos >= 0 && pos < 64)
			{
				e.MovePedra(pos,board,ref m_player);
			}

			}


			base.Update (gameTime);
		}


		void DrawLine(Vector2 start, Vector2 end, Color c)
		{
			primitiveBatch.Begin(PrimitiveType.LineList);

		          // from the nose, down the left hand side
		          primitiveBatch.AddVertex(start, c);
		          primitiveBatch.AddVertex(end, c);

			primitiveBatch.End();
		}

		void DrawRect (Vector2 topLeft, Vector2 size, Color c)
		{
			primitiveBatch.Begin(PrimitiveType.TriangleList);

	          // from the nose, down the left hand side
	        primitiveBatch.AddVertex(topLeft, c);
	        primitiveBatch.AddVertex(new Vector2(topLeft.X + size.X, topLeft.Y), c);
			primitiveBatch.AddVertex(new Vector2(topLeft.X, topLeft.Y + size.Y), c);
			primitiveBatch.AddVertex(new Vector2(topLeft.X + size.X, topLeft.Y), c);
			primitiveBatch.AddVertex(new Vector2(topLeft.X + size.X, topLeft.Y + size.Y), c);
			primitiveBatch.AddVertex(new Vector2(topLeft.X, topLeft.Y + size.Y), c);
			primitiveBatch.End();
		}

		void DrawBoard (Vector2 start, Vector2 size)
		{
			DrawWireBoard (start, size);
			DrawStones (start, size);
		}

		void DrawWireBoard (Vector2 start, Vector2 size)
		{
			int min_size = (int)Math.Min (size.X, size.Y);
			int step = min_size / 8;

			for (int i = 0; i <= min_size; i += step) {
				DrawLine (new Vector2 (start.X, start.Y + i), new Vector2 (start.X + min_size, start.Y + i), Color.LightGreen);
				DrawLine (new Vector2 (start.X + i, start.Y), new Vector2 (start.X + i, start.Y + min_size), Color.LightGreen);
			}
		}

		void DrawStones (Vector2 start, Vector2 size)
		{
			int min_size = (int)Math.Min (size.X, size.Y);
			int step = min_size / 8;

			for(int y = 0; y < 8; y++)
			{

				for(int x = 0; x < 8; x++)
				{
					if (x == xBoard && y == yBoard)	{
						Color color = e.PodeMoverPedra (xBoard + 8*yBoard, board, 1) ? Color.Yellow : Color.YellowGreen; 
						DrawRect (new Vector2 (start.X + x*step, start.Y + y*step), new Vector2 (step, step), color);
					}



					if(e.get(board.p1, x + y*8) != 0)
					{
						DrawRect (new Vector2 (start.X + x*step, start.Y + y*step), new Vector2 (step, step), Color.Red);
					}
					if(e.get(board.p2, x + y*8) != 0)
					{
						DrawRect (new Vector2 (start.X + x*step, start.Y + y*step), new Vector2 (step, step), Color.Blue);
					}
				}
		    }
		}

		/// <summary>
		/// This is called when the game should draw itself.
		/// </summary>
		/// <param name="gameTime">Provides a snapshot of timing values.</param>
		protected override void Draw (GameTime gameTime)
		{
			graphics.GraphicsDevice.Clear (Color.Black);

			int h = graphics.GraphicsDevice.Viewport.Height;
			int w = graphics.GraphicsDevice.Viewport.Width;
			int min = Math.Min(h, w);
			int margin = min/10;
			min = min - margin;

			DrawBoard (new Vector2(margin/2, margin/2), new Vector2 (min, min));
		
			base.Draw (gameTime);

        }
	}
}
