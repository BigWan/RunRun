﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace RunRun {

    public enum MotionStat {
        Stop = 0,
        Speeding = 1,
        Slowing = 2,
        Moveing = 3,
    }

    public delegate void VelocityChangeHandler (float spd);



    /// <summary>
    /// 控制速度值的起步，加速，减速
    /// </summary>
    public class SpeedController : UnitySingleton<SpeedController> {

        /// <summary>
        /// 起步速度
        /// </summary>
        public int startSpeed;

        public float lastSpeed;

        public UnityEvent OnStop;

		/// <summary>
		/// 速度改变的委托
		/// </summary>
		/// <param name="spd">速度值</param>
		public VelocityChangeHandler VelocityChange;



		/// <summary>
		/// 速度的精度误差，如果两个速度之间的差值小于这个值，就判断为速度相等
		/// </summary>
		public static float velocityError = 0.05f;

		/// <summary>
		/// 目标速度
		/// </summary>
		[SerializeField]
		private float _targetVelocity;


		[SerializeField]
		private float _currentVelocity;
		/// <summary>
		/// 当前速度值
		/// </summary>
		/// <value></value>
		public float currentVelocity {
			get { return _currentVelocity; }
			set {
                if (_currentVelocity != value) {
                    _currentVelocity = value;
                    VelocityChange?.Invoke(value);
                }
			}
		}

		/// <summary>
		/// 当前加速度
		/// </summary>
		[SerializeField]
		private float _currentAcceleration;

		/// <summary>
		/// 最小加速度
		/// </summary>
		[SerializeField]
		private float _minAcceleration = 1f;

		/// <summary>
		/// 最大加速度
		/// </summary>
		[SerializeField]
		private float _maxAcceleration = 100f;

		/// <summary>
		/// 标准加速时间
		/// </summary>
		[SerializeField]
		private float _standardAccelerateTime = 0.25f;

		/// <summary>
		/// 是否锁住速度改变
		/// </summary>
		[SerializeField]
		private bool _lockVelocity = false;

        public MotionStat motionStat;

		private void FixedUpdate () {

			if (_lockVelocity) {
                if (currentVelocity == 0) { 
                    motionStat = MotionStat.Stop;
                    OnStop?.Invoke();
                } else {
                    motionStat = MotionStat.Moveing;
                }
            } else {
            
                float deltaSpeed = _targetVelocity - currentVelocity;

                if (Mathf.Abs(deltaSpeed) <= velocityError) {
                    SetCurrentVelocity(_targetVelocity, true);
                    _currentAcceleration = 0;
                    motionStat = MotionStat.Moveing;
                } else {
                    float acc = deltaSpeed / _standardAccelerateTime;

                    if (acc == 0) return;
                    if (acc > 0) {
                        _currentAcceleration = Mathf.Clamp(acc, _minAcceleration, _maxAcceleration);
                        motionStat = MotionStat.Speeding;
                    } else {
                        _currentAcceleration = Mathf.Clamp(acc, -_maxAcceleration, -_minAcceleration);
                        motionStat = MotionStat.Slowing;
                    }
                    currentVelocity += _currentAcceleration * Time.fixedDeltaTime;
                }
            }
		}



		/// <summary>
		/// 设置当前速度
		/// </summary>
		/// <param name="spd">当前速度值</param>
		/// <param name="isLock">是否锁定,默认False代表不锁定，设置完后由引擎自己管理速度值</param>
		public void SetCurrentVelocity (float spd, bool isLock = false) {
			currentVelocity = spd;
			_lockVelocity = isLock;
		}

		/// <summary>
		/// 使目标速度提高 delta
		/// </summary>
		/// <param name="delta">提高值</param>
		public void SpeedUp (float delta) {
			// SetCurrentVelocity (_targetVelocity + delta, false);
			_targetVelocity +=delta;
			_lockVelocity = false;
		}


        public void StartMotion() {
            SpeedTo(startSpeed);
        }

		/// <summary>
		/// 设置目标速度
		/// </summary>
		/// <param name="target">目标速度</param>
		public void SpeedTo(float target){
			_lockVelocity = false;
			_targetVelocity = target;
		}

        public void SpeedBack() {
            SpeedTo(lastSpeed);
        }

		/// <summary>
		/// 保留目标速度，把当前速度设置为0，并锁定。
		/// </summary>
		public void Stop () {
            lastSpeed = currentVelocity;
			SetCurrentVelocity (0, true);
		}


		/// <summary>
		/// 使当前速度值可以被改变
		/// </summary>
		public void UnLock () {
			_lockVelocity = false;
		}

		/// <summary>
		/// 使当前速度值不可被改变
		/// </summary>
		public void Lock () {
			_lockVelocity = false;
		}
	}

}