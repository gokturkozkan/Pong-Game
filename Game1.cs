using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;

namespace Pong
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;
        private Paddle _paddle1;
        private Paddle _paddle2;
        private Ball _ball;
        private Song _startSong;
        private SoundEffect _collisionSound;
        private SoundEffect _scoreSound;
        private Texture2D _middleLineTexture;
        private SpriteFont _font;
        private SpriteFont _font2;
        private SpriteFont _titleFont;
        private int _score1;
        private int _score2;
        private bool _gameStarted;
        private string _startMessage = "\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\n\nPlease insert a coin or press spacebar OR X for AI";
        private string _gameTitle = "PONG\n GAME";
        private double _messageTimer;
        private bool _showMessage;
        private double _countdownTimer;
        private bool _isCountdown;
        private int _countdownValue;
        private bool _gameOver;
        private string _winnerMessage;
        private bool _isAIMode;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;

            _graphics.IsFullScreen = true;
            _graphics.PreferredBackBufferWidth = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width;
            _graphics.PreferredBackBufferHeight = GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height;
            _graphics.ApplyChanges();
            _isAIMode = false;
        }

        protected override void Initialize()
        {
            ResetGame();
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            Texture2D paddleTexture = Content.Load<Texture2D>("PongBar");
            Texture2D ballTexture = Content.Load<Texture2D>("PongBall");
            _startSong = Content.Load<Song>("pong");
            _collisionSound = Content.Load<SoundEffect>("ping_pong_8bit_plop");
            _scoreSound = Content.Load<SoundEffect>("ping_pong_8bit_peeeeeep");
            _middleLineTexture = Content.Load<Texture2D>("PongBar");
            _font = Content.Load<SpriteFont>("ScoreFont");
            _font2 = Content.Load<SpriteFont>("ScoreFont2");
            _titleFont = Content.Load<SpriteFont>("TitleFont");

            _paddle1.LoadContent(paddleTexture);
            _paddle2.LoadContent(paddleTexture);
            _ball.LoadContent(ballTexture);

            MediaPlayer.Play(_startSong);
        }

        private void ResetGame()
        {
            int screenWidth = _graphics.PreferredBackBufferWidth;
            int screenHeight = _graphics.PreferredBackBufferHeight;

            Texture2D paddleTexture = Content.Load<Texture2D>("PongBar");
            Texture2D ballTexture = Content.Load<Texture2D>("PongBall");

            _paddle1 = new Paddle(new Vector2(50, screenHeight / 2 - 50), screenHeight);
            _paddle1.LoadContent(paddleTexture);

            _paddle2 = new Paddle(new Vector2(screenWidth - 70, screenHeight / 2 - 50), screenHeight);
            _paddle2.LoadContent(paddleTexture);
            _paddle2.IsAIControlled = _isAIMode;

            _ball = new Ball(new Vector2(screenWidth / 2, screenHeight / 2), screenWidth, screenHeight, _collisionSound);
            _ball.LoadContent(ballTexture);

            _score1 = 0;
            _score2 = 0;
            _gameStarted = false;
            _messageTimer = 0;
            _showMessage = true;
            _isCountdown = false;
            _countdownTimer = 0;
            _countdownValue = 2;
            _gameOver = false;
            _winnerMessage = "";
        }


        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            var kstate = Keyboard.GetState();

            if (kstate.IsKeyDown(Keys.X))
            {
                _isAIMode = !_isAIMode;
                _paddle2.IsAIControlled = _isAIMode;
            }

            if (!_gameStarted)
            {
                _messageTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_messageTimer >= 1.5)
                {
                    _messageTimer -= 1.5;
                }
                _showMessage = _messageTimer < 1;

                if (_gameOver)
                {
                    if (kstate.IsKeyDown(Keys.Space))
                    {
                        _gameOver = false;
                        _winnerMessage = "";
                        ResetGame();
                        MediaPlayer.Stop();
                        _gameStarted = true;
                        ResetBall();
                    }
                }
                else if (kstate.IsKeyDown(Keys.Space))
                {
                    _gameStarted = true;
                    MediaPlayer.Stop();
                    ResetBall();
                }
            }
            else if (_isCountdown)
            {
                _countdownTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_countdownTimer >= 1)
                {
                    _countdownTimer = 0;
                    _countdownValue--;
                    if (_countdownValue <= 0)
                    {
                        _isCountdown = false;
                        ResetBall();
                    }
                }
            }
            else
            {
                _paddle1.Update(kstate, Keys.W, Keys.S, gameTime);
                if (_isAIMode)
                {
                    _paddle2.UpdateAI(gameTime, _ball.Position);
                }
                else
                {
                    _paddle2.Update(kstate, Keys.Up, Keys.Down, gameTime);
                }
                _ball.Update(gameTime, _paddle1, _paddle2, _collisionSound);

                CheckForScore();
            }

            base.Update(gameTime);
        }


        private void CheckForScore()
        {
            if (_ball.Position.X <= 0)
            {
                _score2++;
                _scoreSound.Play();
                StartCountdown();
            }
            else if (_ball.Position.X >= _graphics.PreferredBackBufferWidth - _ball.Texture.Width)
            {
                _score1++;
                _scoreSound.Play();
                StartCountdown();
            }

            if (_score1 == 3 || _score2 == 3)
            {
                _gameOver = true;
                _gameStarted = false;
                _winnerMessage = _score1 == 3 ? "Player 1 WINS" : "Player 2 WINS";
                MediaPlayer.Play(_startSong);
                _messageTimer = 0;
                _showMessage = true;
            }
        }

        private void StartCountdown()
        {
            _isCountdown = true;
            _countdownValue = 2;
            _countdownTimer = 0;
        }

        private void ResetBall()
        {
            _ball.Position = new Vector2(_graphics.PreferredBackBufferWidth / 2, _graphics.PreferredBackBufferHeight / 2);
            _ball.ResetVelocity();
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();
            _paddle1.Draw(_spriteBatch);
            _paddle2.Draw(_spriteBatch);

            if (!_isCountdown)
            {
                _ball.Draw(_spriteBatch);
            }

            if (_gameStarted && !_isCountdown)
            {
                DrawMiddleLine();
            }

            if (!_gameStarted)
            {
                Vector2 titleSize = _titleFont.MeasureString(_gameTitle);
                Vector2 titlePosition = new Vector2((_graphics.PreferredBackBufferWidth - titleSize.X) / 2, 50);
                _spriteBatch.DrawString(_titleFont, _gameTitle, titlePosition, Color.White);

                if (_showMessage)
                {
                    Vector2 textSize = _font.MeasureString(_startMessage);
                    Vector2 textPosition = new Vector2((_graphics.PreferredBackBufferWidth - textSize.X) / 2, (_graphics.PreferredBackBufferHeight - textSize.Y) / 2);
                    _spriteBatch.DrawString(_font, _startMessage, textPosition, Color.White);
                }

                if (_gameOver)
                {
                    if (_showMessage)
                    {
                        Vector2 winnerSize = _font2.MeasureString(_winnerMessage);
                        Vector2 winnerPosition = new Vector2((_graphics.PreferredBackBufferWidth - winnerSize.X) / 2, (_graphics.PreferredBackBufferHeight - winnerSize.Y) / 2 - 50);
                        _spriteBatch.DrawString(_font2, _winnerMessage, winnerPosition, Color.Yellow);
                    }
                }
            }
            else if (_isCountdown)
            {
                Vector2 countdownSize = _font2.MeasureString(_countdownValue.ToString());
                Vector2 countdownPosition = new Vector2((_graphics.PreferredBackBufferWidth - countdownSize.X) / 2, (_graphics.PreferredBackBufferHeight - countdownSize.Y) / 2);
                _spriteBatch.DrawString(_font2, _countdownValue.ToString(), countdownPosition, Color.White);
            }
            else
            {
                _spriteBatch.DrawString(_font2, _score1.ToString(), new Vector2(_graphics.PreferredBackBufferWidth / 2 - 600, 150), Color.White);
                _spriteBatch.DrawString(_font2, _score2.ToString(), new Vector2(_graphics.PreferredBackBufferWidth / 2 + 520, 150), Color.White);
            }

            _spriteBatch.End();

            base.Draw(gameTime);
        }

        private void DrawMiddleLine()
        {
            int screenHeight = _graphics.PreferredBackBufferHeight;
            int screenWidth = _graphics.PreferredBackBufferWidth;
            int lineHeight = 25;
            int lineCount = 40;
            float lineSpacing = screenHeight / (float)(lineCount + 1);

            for (int i = 0; i < lineCount; i++)
            {
                Vector2 position = new Vector2(screenWidth / 2 - (_middleLineTexture.Width * 0.3f) / 2, lineSpacing * (i + 1) - lineHeight / 2);
                _spriteBatch.Draw(_middleLineTexture, position, null, Color.White, 0f, Vector2.Zero, new Vector2(0.3f, lineHeight / (float)_middleLineTexture.Height), SpriteEffects.None, 0f);
            }
        }
    }

    public class Paddle
    {
        private Texture2D _texture;
        private Vector2 _position;
        private float _speed;
        private int _screenHeight;
        public bool IsAIControlled { get; set; }
        private Vector2 _targetPosition;
        private Random _rand;
        private double _aiReactionDelay;
        private double _aiReactionTimer;

        public Paddle(Vector2 position, int screenHeight)
        {
            _position = position;
            _screenHeight = screenHeight;
            _speed = 800f;
            IsAIControlled = false;
            _targetPosition = position;
            _rand = new Random();
            _aiReactionDelay = 0.027f;
            _aiReactionTimer = 0;
        }

        public void LoadContent(Texture2D texture)
        {
            _texture = texture;
        }

        public void Update(KeyboardState kstate, Keys upKey, Keys downKey, GameTime gameTime)
        {
            if (!IsAIControlled)
            {
                if (kstate.IsKeyDown(upKey))
                {
                    _position.Y -= _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }

                if (kstate.IsKeyDown(downKey))
                {
                    _position.Y += _speed * (float)gameTime.ElapsedGameTime.TotalSeconds;
                }
            }

            _position.Y = MathHelper.Clamp(_position.Y, 0, _screenHeight - (_texture.Height * 4.0f));
        }

        public void UpdateAI(GameTime gameTime, Vector2 ballPosition)
        {
            if (IsAIControlled)
            {
                _aiReactionTimer += gameTime.ElapsedGameTime.TotalSeconds;
                if (_aiReactionTimer >= _aiReactionDelay)
                {
                    _aiReactionTimer -= _aiReactionDelay;

                    _targetPosition.Y = ballPosition.Y - (_texture.Height * 2.0f) + _rand.Next(-40, 41);
                }
                _position.Y = MathHelper.Lerp(_position.Y, _targetPosition.Y, 0.1f);
                _position.Y = MathHelper.Clamp(_position.Y, 0, _screenHeight - (_texture.Height * 4.0f));
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, Color.White, 0f, Vector2.Zero, new Vector2(2.0f, 4.0f), SpriteEffects.None, 0f);
        }

        public bool CheckCollision(Rectangle ballRectangle, ref Vector2 ballVelocity, float initialVelocity, SoundEffect collisionSound)
        {
            Rectangle paddleRectangle = new Rectangle((int)_position.X, (int)_position.Y, _texture.Width * 2, (int)(_texture.Height * 4.0f));

            if (paddleRectangle.Intersects(ballRectangle))
            {
                float hitPosition = (ballRectangle.Center.Y - _position.Y) / (_texture.Height * 4.0f);
                float bounceAngle;

                if (hitPosition < 0.1f)
                {
                    bounceAngle = MathHelper.ToRadians(-45);
                }
                else if (hitPosition > 0.9f)
                {
                    bounceAngle = MathHelper.ToRadians(45);
                }
                else
                {
                    bounceAngle = MathHelper.ToRadians(180 * hitPosition - 90);
                }

                ballVelocity = new Vector2(initialVelocity * (float)Math.Cos(bounceAngle), initialVelocity * (float)Math.Sin(bounceAngle));

                ballVelocity.X *= -1;
                ballVelocity *= 1.1f;

                if (ballRectangle.Center.X < paddleRectangle.Center.X)
                {
                    ballVelocity.X = Math.Abs(ballVelocity.X);
                }
                else
                {
                    ballVelocity.X = -Math.Abs(ballVelocity.X);
                }

                collisionSound.Play();

                return true;
            }

            return false;
        }
    }
    public class Ball
    {
        private Texture2D _texture;
        private Vector2 _position;
        private Vector2 _velocity;
        private int _screenWidth;
        private int _screenHeight;
        private float _initialVelocity;
        private SoundEffect _collisionSound;

        public Ball(Vector2 position, int screenWidth, int screenHeight, SoundEffect collisionSound)
        {
            _position = position;
            _screenWidth = screenWidth;
            _screenHeight = screenHeight;
            _initialVelocity = 850f;
            _collisionSound = collisionSound;
            ResetVelocity();
        }

        public void LoadContent(Texture2D texture)
        {
            _texture = texture;
        }

        public void ResetVelocity()
        {
            Random rand = new Random();
            float angle;

            int angleRange = rand.Next(2);
            if (angleRange == 0)
            {
                angle = MathHelper.ToRadians(rand.Next(30, 60));
            }
            else
            {
                angle = MathHelper.ToRadians(rand.Next(120, 150));
            }

            _velocity = new Vector2(_initialVelocity * (float)Math.Cos(angle), _initialVelocity * (float)Math.Sin(angle));
        }

        public void Update(GameTime gameTime, Paddle paddle1, Paddle paddle2, SoundEffect collisionSound)
        {
            _position += _velocity * (float)gameTime.ElapsedGameTime.TotalSeconds;
            _collisionSound = collisionSound;

            if (_position.Y <= 0)
            {
                _position.Y = 0;
                _velocity.Y *= -1;
                _collisionSound.Play();
            }
            else if (_position.Y >= _screenHeight - _texture.Height * 4.0f)
            {
                _position.Y = _screenHeight - _texture.Height * 4.0f;
                _velocity.Y *= -1;
                _collisionSound.Play();
            }

            Rectangle ballRectangle = new Rectangle((int)_position.X, (int)_position.Y, (int)(_texture.Width * 4.0f), (int)(_texture.Height * 4.0f));

            if (paddle1.CheckCollision(ballRectangle, ref _velocity, _initialVelocity, _collisionSound) || paddle2.CheckCollision(ballRectangle, ref _velocity, _initialVelocity, _collisionSound))
            {
                _velocity.X *= -1;
            }
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(_texture, _position, null, Color.White, 0f, Vector2.Zero, 4.0f, SpriteEffects.None, 0f);
        }

        public Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }

        public Texture2D Texture
        {
            get { return _texture; }
        }
    }
}
