#region LGPL License
/*
 * Physics 2D is a 2 Dimensional Rigid Body Physics Engine written in C#. 
 * For the latest info, see http://physics2d.sourceforge.net/
 * Copyright (C) 2005-2006  Jonathan Mark Porter
 * 
 * This library is free software; you can redistribute it and/or
 * modify it under the terms of the GNU Lesser General Public
 * License as published by the Free Software Foundation; either
 * version 2.1fof the License, or (at your option) any later version.
 * 
 * This library is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
 * Lesser General Public License for more details.
 * 
 * You should have received a copy of the GNU Lesser General Public
 * License along with this library; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301, USA
 * 
 */
#endregion
using System;
using Physics2D;
//using Physics2D.Collision;
using AdvanceMath.Geometry2D;
using AdvanceMath;
namespace WindowsDriver.Demos.HyperMeleeDemo
{


    public interface ITracker : IUpdatable
    {
        float MaxAccel { get;}
        float MaxVelocity { get;}
        float Orientation { get;}
        float Desired { get;set;}
        bool OnTarget { get;}
    }
    public class Tracker : ITracker
    {
        float maxTorque;
        float maxVelocity;
        ICollidableBody trackie;
        ICollidableBody target;
        float desired;
        bool onTarget = false;
        public Tracker(ICollidableBody target, ICollidableBody trackie, float maxTorque, float maxVelocity)
        {
            this.target = target;
            this.trackie = trackie;
            this.maxTorque = maxTorque;
            this.maxVelocity = maxVelocity;
        }
        public float MaxAccel
        {
            get
            {
                return maxTorque * trackie.MassInertia.MomentofInertiaInv;
            }
        }

        public float MaxVelocity
        {
            get
            {
                return maxVelocity;
            }
        }

        public float Orientation
        {
            get
            {
                return MathAdv.RadianMin(trackie.Current.Position.Angular);
            }
        }

        public float Desired
        {
            get
            {
                return desired;
            }
            set
            {
                desired = MathAdv.RadianMin(value);
            }
        }

        public bool OnTarget
        {
            get
            {
                return onTarget;
            }
        }


        public void Update(float dt)
        {
            onTarget = false;
            Vector2D DifferenceL = target.Current.Position.Linear - trackie.Current.Position.Linear;
            Desired = DifferenceL.Angle;
            float DifferenceA = MathAdv.GetAngleDifference(Desired, Orientation);
            float Velocity = trackie.Current.Velocity.Angular;
            float AbsVelocity = (float)Math.Abs(Velocity);
            if (AbsVelocity > MaxVelocity)
            {
                float dv = (float)Math.Sign(Velocity) * ((float)Math.Abs(Velocity) - MaxVelocity);
                if ((float)Math.Abs(dv) > MaxAccel * dt)
                {
                    dv = (float)Math.Sign(Velocity) * MaxAccel * dt;
                }
                trackie.Current.Velocity.Angular -= dv;
                return;
            }
            float Accel = MaxAccel * dt;
            if ((float)Math.Abs(DifferenceA) > (AbsVelocity * AbsVelocity )/(2*MaxAccel))
            {
                if (DifferenceA > 0)
                {
                    trackie.Current.Velocity.Angular += Accel;
                }
                else
                {
                    trackie.Current.Velocity.Angular -= Accel;
                }
            }
            else
            {
                float dv = Velocity;
                if ((float)Math.Abs(dv) > MaxAccel * dt)
                {
                    dv = (float)Math.Sign(Velocity) * MaxAccel * dt;
                }
                trackie.Current.Velocity.Angular -= dv;
                onTarget = true;
                return;
            }
        }
    }





    interface IMissle : IUpdatable 
    {
        ICollidableBody Target { get;set;}
        ICollidableBody Source { get; set;}
        ICollidableBody Missile { get; set;}

        float MaxThrust { get;}
        float MaxTorque { get;}
        float MaxVelocity { get;}
    }

    class Missile
    {
        public ICollidableBody target;
        public ICollidableBody missile;
        Tracker tracker;
        //float maxTorque;

        float maxThrust;
        float maxLV;
        public Missile(ICollidableBody target, ICollidableBody missile, float maxTorque,float maxAV, float maxThrust,float maxLV)
        {
            tracker = new Tracker(target, missile, maxTorque, maxAV);
            this.target = target;
            this.missile = missile;
            this.maxThrust = maxThrust;
            this.maxLV = maxLV;
            //this.maxTorque = maxTorque;
        }
        public void Update(float dt)
        {
            tracker.Update(dt);

            if (tracker.OnTarget)
            {
                float dVt = maxThrust * dt * missile.MassInertia.MassInv;

                //Vector2D DifferenceL = target.Current.Position.Linear - missile.Current.Position.Linear;
                //DifferenceL = DifferenceL.Normalized;
                Vector2D DifferenceL = Vector2D.FromLengthAndAngle(1, missile.Current.Position.Angular);
                if (missile.Current.Velocity.Linear * DifferenceL < maxLV)
                {
                    missile.Current.Velocity.Linear += (DifferenceL * dVt);
                }
                Vector2D TangetL = DifferenceL.RightHandNormal;
                float tangentvel = missile.Current.Velocity.Linear * TangetL;
                if ((float)Math.Abs(tangentvel) > dVt)
                {
                    missile.Current.Velocity.Linear -= (float)Math.Sign(tangentvel) * dVt * TangetL;
                }
                else
                {
                    missile.Current.Velocity.Linear -= tangentvel * TangetL;
                }
            }
            else
            {
                float dVt = maxThrust * dt * missile.MassInertia.MassInv*.01f;

                Vector2D vel = missile.Current.Velocity.Linear.Normalized;

                float velmag = missile.Current.Velocity.Linear * vel;
                if ((float)Math.Abs(velmag) > dVt)
                {
                    missile.Current.Velocity.Linear -= (float)Math.Sign(velmag) * dVt * vel;
                }
                else
                {
                    missile.Current.Velocity.Linear -= velmag * vel;
                }
            }
        }
    }
}
