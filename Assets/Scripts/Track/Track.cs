﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace RunRun {

    /// <summary>
    /// 跑道
    /// </summary>
    public class Track : UnitySingleton<Track> {

        [Header("Section Data")]

        /// <summary>
        /// 当前track的section配置表集合
        /// </summary>
        public RoadSectionData[] datas;

        /// <summary>
        /// 开始和结束的固定块
        /// </summary>
        public RoadSectionData 
            startData,
            endData;




        [Header("Prefab Ref")]
        public RoadSection sectionPrefab;


        [Header("Config")]
        /// <summary>
        /// 跑道长度限制
        /// </summary>
        public float maxLength;

        /// <summary>
        /// 出现金币的概率(每个Block出现金币的概率)
        /// </summary>
        public float coinRate = 0.5f;

        [Header("Info")]
        /// <summary>
        /// 跑道长度
        /// </summary>
        public float currentLength;




        /// <summary>
        /// 段落
        /// </summary>
        private List<RoadSection> sections;

        [SerializeField]
        private Vector3 currentPosition;
        [SerializeField]
        private Quaternion currentRotation;

        private void Awake() {
            Init();
        }

        private void Start() {
            PreSpawn();
        }


        public void PreSpawn() {
            // 提前生成若干格块
            int preSpawnCount = 2;
            for (int i = 0; i < preSpawnCount; i++) {
                SpawnNextSection();
            }
        }

        public void Init() {
            // 重置引用列表
            if(sections == null)
                sections = new List<RoadSection>();
            if (sections.Count > 0) {
                foreach (var section in sections) {
                    Destroy(section.gameObject);
                }
                sections.Clear();
            }
            currentLength = 0;
        }



        void SpawnOneSection(RoadSectionData data,float coinRate = 0,bool isEnd = false) {
            RoadSection section = Instantiate<RoadSection>(sectionPrefab) as RoadSection;
            section.SetData(data);
            section.transform.SetParent(transform);
            section.transform.localPosition = currentPosition;
            section.transform.localRotation = currentRotation;
            section.Execute(coinRate);

            Vector3 endpos = (maxLength - currentLength) * new Vector3(0, 0, 1);

            if (isEnd) {
                section.SpawnEnd(endpos.z);
            }

            currentRotation = section.transform.localRotation * section.getEndRoation();
            currentPosition = section.transform.localPosition + section.getEndRoation() * section.getEndPosition();

            currentLength += section.getLength();
        }



        private bool finished;

        /// <summary>
        /// 单步生成
        /// </summary>        
        public void SpawnNextSection() {
            if (finished) return;
            if (Mathf.Approximately(currentLength, 0)) {
                SpawnOneSection(startData, 0);
                return;
            }

            if (currentLength < maxLength - 10) {
                int rndIndex = Random.Range(0, datas.Length);
                RoadSectionData data = datas[rndIndex];
                SpawnOneSection(data, coinRate);
                return;
            } else {
                finished = true;
                SpawnOneSection(endData, 0,true);
            }
        }

        /// <summary>
        /// 生成所有
        /// </summary>
        /// <param name="coinRate"></param>
        public void SpawnAllSections(float coinRate = 0) {

            SpawnOneSection(startData,0);
            while (currentLength < maxLength-20) {
                int rndIndex = Random.Range(0, datas.Length);   
                RoadSectionData data = datas[rndIndex];
                SpawnOneSection(data,coinRate);
            }

            SpawnOneSection(endData, 0);           
        }

        //#if UNITY_EDITOR

        //private void OnGUI() {
        //    if(GUI.Button(new Rect(30, 30, 100, 30), "Spawn")) {
        //        //SpawnAllSections(coinRate);
        //        SpawnNextSection();
        //    }
        //}

        //#endif

    }
}
