using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using Microsoft.Xna.Framework.Storage;
using FarseerPhysics.Common;
using FarseerPhysics.Collision;
using FarseerPhysics.DebugViews;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;

namespace Platformer
{
    public class Player : DynamicObject
    {
        protected GamePadState _gamePadState;
        protected GamePadState _gamePadStatePrevious;

        protected Color Colour;

        protected IPickable _heldObject;

        protected MoveState _moveState;
        protected float _jumpTimer = 0;
        public float _jumpPower = 0;
        protected bool _jumpPressed = true;

        protected Fixture _body;
        protected Fixture _wheel;
        protected FarseerPhysics.Dynamics.Joints.FixedAngleJoint _wheelBreak;
        public List<DynamicObject> _dynamicObjects;

        protected Health _sharedHealth;

        protected SoundEffect _jumpSound;
        protected SoundEffect _dragSound;
        protected SoundEffect _bounceSound;

        protected double _invincibility = 0;

        public Player()
            : base()
        {
            CurrentScreen = new Vector2(-1, -1);
            Direction = Direction.LEFT;
            ClimbableThings = new List<Fixture>();
            Collectables = Collectables | Collectable.JUMP;
        }

        public override void Create(World world, float Xoffset)
        {
            _jumpSound = _content.Load<SoundEffect>("Sounds\\jump");
            _dragSound = _content.Load<SoundEffect>("Sounds\\drag");
            _bounceSound = _content.Load<SoundEffect>("Sounds\\smash");
            base.Create(world, Xoffset);
            _moveState = MoveState.AIR;
            _body = FixtureFactory.CreateRectangle(_world, 1, 1.5f, 1, Position);
            _body.Body.BodyType = BodyType.Dynamic;
            _body.Restitution = 0.3f;
            _body.Friction = 0.5f;
            _joints.Add(JointFactory.CreateFixedAngleJoint(_world, _body.Body));
            _wheel = FixtureFactory.CreateCircle(_world, 1 / 2.0f, 1, _body.Body.Position + new Vector2(0, -(0.5f * 1.5f)));
            _wheel.Body.BodyType = BodyType.Dynamic;
            _joints.Add(JointFactory.CreateRevoluteJoint(_world, _body.Body, _wheel.Body, Vector2.Zero));
            _body.CollisionFilter.IgnoreCollisionWith(_wheel);
            _wheel.CollisionFilter.IgnoreCollisionWith(_body);
            _wheel.Friction = 1.0f;
            _fixtures.Add(_body);
            _fixtures.Add(_wheel);
            _body.UserData = this;
            _wheel.UserData = this;

            // Create a break for when the player isn't moving so friction can act on the player properly
            _wheelBreak = JointFactory.CreateFixedAngleJoint(_world, _wheel.Body);
            _wheelBreak.Enabled = true;

            _body.OnCollision += new OnCollisionEventHandler(OnCollision);
            _body.OnSeparation += new OnSeparationEventHandler(OnSeparation);
            
            _wheel.OnCollision += new OnCollisionEventHandler(OnCollision);
            _wheel.OnSeparation += new OnSeparationEventHandler(OnSeparation);
        }

        public override void Dispose()
        {
            base.Dispose();
            if (_dynamicObjects != null)
            {
                foreach (DynamicObject dObject in _dynamicObjects)
                {
                    if (dObject is Bullet)
                    {
                        ((Bullet)dObject).Dispose();
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
//            if (SavePointContact && _gamePadState.Buttons.Y == ButtonState.Released && _gamePadStatePrevious.Buttons.Y == ButtonState.Pressed)
//            {
//                PlayerLoad player;
//                if (PlayerIndex == PlayerIndex.One)
//                {
//                    player = SaveGame.PlayerOne;
//                }
//                else
//                {
//                    player = SaveGame.PlayerTwo;
//                }

//                player.NextScreen = NextScreen;
//                player.Position = Position;
//                Collectables = Collectable.JUMP;
//                player.Collectables = Collectables;
//                SaveGame.Health = MaxHealth;

//#if (XBOX360)
//#else
//#endif

//                //FileStream fileStream = new FileStream(SaveGame.FileName, FileMode.Create);
//                //XmlSerializer xs = new XmlSerializer(SaveGame.GetType());
//                //xs.Serialize(fileStream, SaveGame);
//                //fileStream.Close();
//            }
            if (_fixtures.Count > 0)
            {
                if (_heldObject != null)
                {
                    if (_gamePadState.Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        // While the button is pressed, update held object
                        _heldObject.SetHeld(X, Y, _fixtures[0].Body.LinearVelocity, Direction);
                    }
                    else if (_gamePadStatePrevious.Buttons.LeftShoulder == ButtonState.Pressed)
                    {
                        // Once it's released
                        _heldObject.Release();
                        _heldObject = null;
                    }
                }

                if (ClimbableThings.Count > 0 && (Collectables & Collectable.GRAB) == Collectable.GRAB)
                {
                    MoveState = MoveState.GROUND;
                }

                if (_gamePadState.IsButtonDown(Buttons.A))
                {
                    if ((Collectables & Collectable.JUMP) == Collectable.JUMP || ((Collectables & Collectable.GRAB) == Collectable.GRAB && ClimbableThings.Count > 0))
                    {
                        if (MoveState == MoveState.GROUND && _jumpPressed == false)
                        {
                            MainFixture.Body.ApplyForce(new Vector2(0, 120.0f * (float)gameTime.ElapsedGameTime.TotalMilliseconds));
                            MoveState = MoveState.AIR;
                            _jumpPressed = true;
                            _jumpPower = Math.Abs(MainFixture.Body.LinearVelocity.X);
                            _jumpTimer = 0;
                            if ((Collectables & Collectable.JUMP) == Collectable.JUMP)
                                _jumpSound.Play(0.5f, 0.0f, 0.0f);
                            else
                                _dragSound.Play(0.5f, 0.0f, 0.0f);
                        }
                        else if (_jumpTimer < 150)
                        {
                            MainFixture.Body.ApplyForce(new Vector2(0, _jumpPower * (float)(gameTime.ElapsedGameTime.TotalMilliseconds) * 0.2f));
                        }
                        _jumpTimer += gameTime.ElapsedGameTime.Milliseconds;
                    }
                    if ((Collectables & Collectable.GRAB) == Collectable.GRAB && ClimbableThings.Count > 0)
                    {
                        if (MainFixture.Body.LinearVelocity.Y > 10)
                            MainFixture.Body.LinearVelocity = new Vector2(MainFixture.Body.LinearVelocity.X, 10);
                    }
                }
                else
                {
                    _jumpPressed = false;

                    if (Math.Abs(_body.Body.LinearVelocity.Y) <= 0.0001)
                        _moveState = MoveState.GROUND;
                }
                if (_gamePadState.ThumbSticks.Left.X < -0.5 || _gamePadState.ThumbSticks.Left.X > 0.5)
                {
                    if (!(_moveState == MoveState.AIR && _jumpPower == 0))
                    {
                        _wheelBreak.Enabled = false;
                        if (_gamePadState.ThumbSticks.Left.X < -0.5)
                        {
                            Direction = Direction.LEFT;
                            _wheel.Body.ApplyForce(new Vector2(-3.0f * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0));
                            MainFixture.Body.ApplyForce(new Vector2(-3.0f * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0));
                            if (MainFixture.Body.LinearVelocity.X < -12)
                            {
                                MainFixture.Body.LinearVelocity = new Vector2(-12, MainFixture.Body.LinearVelocity.Y);
                            }
                        }
                        if (_gamePadState.ThumbSticks.Left.X > 0.5)
                        {
                            Direction = Direction.RIGHT;
                            _wheel.Body.ApplyForce(new Vector2(3.0f * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0));
                            MainFixture.Body.ApplyForce(new Vector2(3.0f * (float)gameTime.ElapsedGameTime.TotalMilliseconds, 0));
                            if (MainFixture.Body.LinearVelocity.X > 12)
                            {
                                MainFixture.Body.LinearVelocity = new Vector2(12, MainFixture.Body.LinearVelocity.Y);
                            }
                        }
                        if (MoveState == MoveState.AIR)
                        {
                            if (MainFixture.Body.LinearVelocity.X < -_jumpPower - 3)
                            {
                                MainFixture.Body.LinearVelocity = new Vector2(-_jumpPower - 3, MainFixture.Body.LinearVelocity.Y);
                            }
                            else if (MainFixture.Body.LinearVelocity.X > _jumpPower + 3)
                            {
                                MainFixture.Body.LinearVelocity = new Vector2(_jumpPower + 3, MainFixture.Body.LinearVelocity.Y);
                            }
                        }
                    }
                }
                else
                {
                    // Enable the wheel break so we don't roll off forever
                    _wheelBreak.TargetAngle = _fixtures[1].Body.Rotation;
                    _wheelBreak.Enabled = true;
                }

                if (_invincibility <= 0)
                {
                    foreach (Fixture fixture in _contactFixtures)
                    {
                        if (fixture.UserData is IHurty)
                        {
                            SharedHealth.CurrentHealth -= ((IHurty)fixture.UserData).GetDamage();
                            _invincibility = 2000;
                        }
                    }
                }
                else
                {
                    _invincibility -= gameTime.ElapsedGameTime.TotalMilliseconds;
                }
            }

            base.Update(gameTime);
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            if (_invincibility <= 0 || gameTime.TotalGameTime.Milliseconds % 250 < 150)
            {
                spriteBatch.Draw(_textures[3], new Vector2((_fixtures[1].Body.Position.X * 32) + 640, 360 - (_fixtures[1].Body.Position.Y * 32)), null, Color.White, -_fixtures[1].Body.Rotation, new Vector2(16, 16), 1, SpriteEffects.None, 0);
                spriteBatch.Draw(_textures[2], new Vector2((Position.X * 32) + 640, 360 - (Position.Y * 32)), null, Colour, 0, new Vector2(16, 24), 1, SpriteEffects.None, 0);
            }
        }

        public bool OnCollision(Fixture fix1, Fixture fix2, Contact contact)
        {
            if (fix2.UserData is Bullet && !((Bullet)fix2.UserData).Player && _invincibility <= 0)
            {
                SharedHealth.CurrentHealth -= fix2.Body.LinearVelocity.Length();
                ((Bullet)fix2.UserData).Dispose();
                _invincibility = 1500;
            }
            if (fix2.UserData is Spike)
            {
                if (((Spike)fix2.UserData).MainFixture.Body.BodyType == BodyType.Static)
                {
                    ((Spike)fix2.UserData).MainFixture.Body.BodyType = BodyType.Dynamic;
                }
            }
            if (fix2.UserData is PickupSpecial)
            {
                PickupSpecial pickup = (PickupSpecial)fix2.UserData;

                if (pickup.PlayerIndex == PlayerIndex || pickup.PlayerIndex == PlayerIndex.Three)
                {
                    switch (pickup.GameSwitch)
                    {
                        default:
                            GameSave.CurrentGameSwitches |= pickup.GameSwitch;
                            pickup.Dead = true;
                            break;
                    }
                }
            }
            if (fix2.UserData is Enemy && ((Enemy)fix2.UserData).TouchDamage > 0 && _invincibility <= 0)
            {
                _sharedHealth.CurrentHealth -= ((Enemy)fix2.UserData).TouchDamage;
                _invincibility = 1500;
            }
            if (fix2.UserData is Pickup)
            {
                Pickup pickup = (Pickup)fix2.UserData;

                if (pickup.PlayerIndex == PlayerIndex || pickup.PlayerIndex == PlayerIndex.Three)
                {
                    switch (pickup.Collectable)
                    {
                        case Collectable.HEALTH:
                            SharedHealth.CurrentHealth += 25;
                            pickup.Dead = true;
                            break;
                        case Collectable.FULLHEALTH:
                            SharedHealth.CurrentHealth = SharedHealth.MaxHealth;
                            pickup.Dead = true;
                            break;
                        default:
                            Collectables |= pickup.Collectable;
                            pickup.Dead = true;
                            break;
                    }
                }
            }

            //if ((Collectables & Collectable.GRAB) != Collectable.GRAB)
            //    _jumpPower = 0;
            if (_body.Body.LinearVelocity.Length() > 20)
            {
                Health -= _body.Body.LinearVelocity.Length() / 2.0;
                _invincibility = 1500;
                _bounceSound.Play(0.5f, 0, 0);
            }

            if (fix2.UserData is IPickable && _gamePadState.Buttons.LeftShoulder == ButtonState.Pressed && _heldObject == null && (Collectables & Collectable.GRAB) == Collectable.GRAB)
            {
                _heldObject = (IPickable)fix2.UserData;
            }
            //else if (fix2.UserData is IClibmableThing)
            //{
            //    ClimbableThings.Add(fix2);
            //}
            else if (fix2.UserData is IPassableBottom)
            {
                if (_fixtures[0].Body.LinearVelocity.Y > 0 || _fixtures[1].Body.Position.Y - 0.3 < fix2.Body.Position.Y)
                    return false;
            }
            else if (!fix2.IsSensor)
            {
                // Check if the player is standing on a surface at less than pi/4 radians from the horizontal
                // First calculate the overall normal
                Vector2 normal = Vector2.Zero;

                Contact currentContact = contact;
                Vector2 nextNormal = Vector2.Zero;
                FixedArray2<Vector2> fx; // No idea what that is, but the function wants it lol
                // Iterate through the contacts, summing the normals
                do
                {
                    Vector2 vec = Vector2.Zero;
                    contact.GetWorldManifold(out vec, out fx);
                    normal += vec;
                    currentContact = currentContact.Next;
                } while (currentContact != null);

                double normalAngle = Math.Atan2(normal.X, normal.Y);

                if (Math.Abs(normalAngle) <= MathHelper.PiOver4)
                {
                    _moveState = MoveState.GROUND;
                }
                else if (Math.Abs(normalAngle) < MathHelper.Pi)
                {
                    _moveState = MoveState.SLIDING;
                }
            }

            return true;
        }

        public void OnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
        //    if (fixtureB.UserData is IClibmableThing)
        //    {
        //        ClimbableThings.Remove(fixtureB);
        //    }
            _moveState = MoveState.AIR;
        }
        
        public IPickable HeldObject
        {
            set
            {
                _heldObject = null;
            }
        }

        public List<Fixture> ClimbableThings
        {
            get;
            set;
        }

        public override Fixture MainFixture
        {
            get
            {
                return _body;
            }
        }

        public Vector2 CurrentScreen
        {
            get;
            set;
        }

        public Vector2 NextScreen
        {
            get;
            set;
        }

        public bool SavePointContact
        {
            get;
            set;
        }

        public VirtualScreen VirtualScreen
        {
            get;
            set;
        }

        public Direction Direction
        {
            get;
            set;
        }

        public MoveState MoveState
        {
            get
            {
                return _moveState;
            }
            set
            {
                _moveState = value;
            }
        }

        public PlayerIndex PlayerIndex
        {
            get;
            set;
        }

        public Collectable Collectables
        {
            get;
            set;
        }

        // TODO: make this not cyclic
        public SaveGame SaveGame
        {
            get;
            set;
        }

        public override Vector2 Position
        {
            get
            {
                if (_fixtures.Count <= 0)
                    return _position;
                else
                    return _fixtures[0].Body.Position;
            }
            set
            {
                if (_fixtures.Count <= 0)
                    _position = value;
                else
                {
                    Vector2 difference = value - _fixtures[0].Body.Position;
                    foreach (Fixture fixture in _fixtures)
                    {
                        fixture.Body.Position += difference;
                    }
                }
            }
        }

        public Health SharedHealth
        {
            get
            {
                return _sharedHealth;
            }
            set
            {
                if (Initialised)
                    _sharedHealth = value;
            }
        }

        public override double Health
        {
            get
            {
                return _sharedHealth.CurrentHealth;
                
            }
            set
            {
                if (Initialised)
                    _sharedHealth.CurrentHealth = value;
            }
        }

        public override double MaxHealth
        {
            get
            {
                return _sharedHealth.MaxHealth;
            }
            set
            {
                if (Initialised)
                    _sharedHealth.MaxHealth = value;
            }
        }
    }

    public enum MoveState
    {
        GROUND,
        SLIDING,
        AIR
    }

    public enum VirtualScreen
    {
        ONE,
        TWO
    }
}
