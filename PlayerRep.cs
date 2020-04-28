﻿using Facepunch.Steamworks;
using MelonLoader;
using RootMotion;
using RootMotion.FinalIK;
using StressLevelZero.Rig;
using StressLevelZero.VFX;
//using RootMotion.FinalIK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using static UnityEngine.Object;

namespace MultiplayerMod
{
    public class PlayerRep
    {
        public static bool hideBody = false;

        public GameObject ford;
        public GameObject head;
        public GameObject handL;
        public GameObject handR;
        public GameObject pelvis;
        public GameObject nametag;
        public GameObject footL;
        public GameObject footR;
        public IKSolverVR.Arm lArm;
        public IKSolverVR.Arm rArm;
        public IKSolverVR.Spine spine;
        public VRIK ik;
        public GameObject namePlate;
        public SteamId steamId;
        public BoneworksRigTransforms rigTransforms;
        public GameObject currentGun;
        public GameObject gunParent;

        private static AssetBundle fordBundle;

        public static void LoadFord()
        {
            fordBundle = AssetBundle.LoadFromFile("ford.ford");
            if (fordBundle == null)
                MelonModLogger.LogError("Failed to load Ford asset bundle");

            GameObject fordPrefab = fordBundle.LoadAsset("Assets/brett_body.prefab").Cast<GameObject>();
            if (fordPrefab == null)
                MelonModLogger.LogError("Failed to load Ford from the asset bundle???");
        }

        public PlayerRep(string name, SteamId steamId)
        {
            this.steamId = steamId;
            GameObject ford = Instantiate(fordBundle.LoadAsset("Assets/Ford.prefab").Cast<GameObject>());

            // attempt to fix shaders
            foreach (SkinnedMeshRenderer smr in ford.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                foreach (Material m in smr.sharedMaterials)
                {
                    m.shader = Shader.Find("Valve/vr_standard");
                }
            }

            DontDestroyOnLoad(ford);

            GameObject root = ford.transform.Find("Ford/Brett@neutral").gameObject;
            Transform realRoot = root.transform.Find("SHJntGrp/MAINSHJnt/ROOTSHJnt");

            //var ik = root.AddComponent<VRIK>();

            //VRIK.References bipedReferences = new VRIK.References
            //{
            //    root = root.transform.Find("SHJntGrp"),

            //    spine = realRoot.Find("Spine_01SHJnt"),
            //    pelvis = realRoot,

            //    leftThigh = realRoot.Find("l_Leg_HipSHJnt"),
            //    leftCalf = realRoot.Find("l_Leg_HipSHJnt/l_Leg_KneeSHJnt"),
            //    leftFoot = realRoot.Find("l_Leg_HipSHJnt/l_Leg_KneeSHJnt/l_Leg_AnkleSHJnt"),
            //    leftToes = realRoot.Find("l_Leg_HipSHJnt/l_Leg_KneeSHJnt/l_Leg_AnkleSHJnt/l_Leg_BallSHJnt"),

            //    rightThigh = realRoot.Find("r_Leg_HipSHJnt"),
            //    rightCalf = realRoot.Find("r_Leg_HipSHJnt/r_Leg_KneeSHJnt"),
            //    rightFoot = realRoot.Find("r_Leg_HipSHJnt/r_Leg_KneeSHJnt/r_Leg_AnkleSHJnt"),
            //    rightToes = realRoot.Find("r_Leg_HipSHJnt/r_Leg_KneeSHJnt/r_Leg_AnkleSHJnt/r_Leg_BallSHJnt"),

            //    leftUpperArm = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/l_Arm_ClavicleSHJnt/l_AC_AuxSHJnt/l_Arm_ShoulderSHJnt"),
            //    leftForearm = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/l_Arm_ClavicleSHJnt/l_AC_AuxSHJnt/l_Arm_ShoulderSHJnt/l_Arm_Elbow_CurveSHJnt"),
            //    leftHand = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/l_Arm_ClavicleSHJnt/l_AC_AuxSHJnt/l_Arm_ShoulderSHJnt/l_Arm_Elbow_CurveSHJnt/l_WristSHJnt/l_Hand_1SHJnt"),
            //    //leftShoulder = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/l_Arm_ClavicleSHJnt/l_AC_AuxSHJnt/l_Arm_ShoulderSHJnt"),

            //    rightUpperArm = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/r_Arm_ClavicleSHJnt/r_AC_AuxSHJnt/r_Arm_ShoulderSHJnt"),
            //    rightForearm = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/r_Arm_ClavicleSHJnt/r_AC_AuxSHJnt/r_Arm_ShoulderSHJnt/r_Arm_Elbow_CurveSHJnt"),
            //    rightHand = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/r_Arm_ClavicleSHJnt/r_AC_AuxSHJnt/r_Arm_ShoulderSHJnt/r_Arm_Elbow_CurveSHJnt/r_WristSHJnt/r_Hand_1SHJnt"),
            //    //rightShoulder = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/r_Arm_ClavicleSHJnt/r_AC_AuxSHJnt/r_Arm_ShoulderSHJnt"),

            //    head = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/Neck_01SHJnt")
            //};

            ////ik.AutoDetectReferences();
            //ik.enabled = false;
            //ik.references = bipedReferences;
            ////ik.solver.plantFeet = false;
            //ik.fixTransforms = false;

            //ik.solver.leftLeg.positionWeight = 1.0f;
            //ik.solver.leftLeg.swivelOffset = 0.0f;
            //ik.solver.leftLeg.bendToTargetWeight = 0.0f;
            //ik.solver.leftLeg.legLengthMlp = 1.0f;

            //ik.solver.rightLeg.positionWeight = 1.0f;
            //ik.solver.rightLeg.swivelOffset = 0.0f;
            //ik.solver.rightLeg.bendToTargetWeight = 0.0f;
            //ik.solver.rightLeg.legLengthMlp = 1.0f;

            //ik.solver.hasChest = false;
            //ik.solver.spine.chestGoalWeight = 0.0f;
            //ik.solver.spine.pelvisPositionWeight = 0.0f;
            //ik.solver.spine.pelvisRotationWeight = 0.0f;
            //ik.solver.spine.positionWeight = 1.0f;
            //ik.solver.spine.rotationWeight = 1.0f;
            //ik.solver.spine.minHeadHeight = 0.8f;
            //ik.solver.spine.bodyPosStiffness = 0.55f;
            //ik.solver.spine.bodyRotStiffness = 0.1f;
            //ik.solver.spine.neckStiffness = 0.3f;
            //ik.solver.spine.rotateChestByHands = 1.0f;
            //ik.solver.spine.chestClampWeight = 0.5f;
            //ik.solver.spine.headClampWeight = 0.6f;
            //ik.solver.spine.moveBodyBackWhenCrouching = 0.5f;
            //ik.solver.spine.maintainPelvisPosition = 0.2f;
            //ik.solver.spine.maxRootAngle = 25.0f;

            //ik.solver.leftArm.positionWeight = 1.0f;
            //ik.solver.leftArm.rotationWeight = 1.0f;
            //ik.solver.leftArm.shoulderRotationMode = IKSolverVR.Arm.ShoulderRotationMode.YawPitch;
            //ik.solver.leftArm.shoulderRotationWeight = 0.5f;
            //ik.solver.leftArm.shoulderTwistWeight = 1.0f;
            //ik.solver.leftArm.bendGoalWeight = 0.0f;
            //ik.solver.leftArm.swivelOffset = 20.0f;
            //ik.solver.leftArm.armLengthMlp = 1.0f;

            //ik.solver.rightArm.positionWeight = 1.0f;
            //ik.solver.rightArm.rotationWeight = 1.0f;
            //ik.solver.rightArm.shoulderRotationMode = IKSolverVR.Arm.ShoulderRotationMode.YawPitch;
            //ik.solver.rightArm.shoulderRotationWeight = 0.5f;
            //ik.solver.rightArm.shoulderTwistWeight = 1.0f;
            //ik.solver.rightArm.bendGoalWeight = 0.0f;
            //ik.solver.rightArm.swivelOffset = 20.0f;
            //ik.solver.rightArm.armLengthMlp = 1.0f;

            //IKSolverVR.Locomotion l = ik.solver.locomotion;
            //l.weight = 0.0f;
            //l.blockingEnabled = false;
            //l.blockingLayers = LayerMask.NameToLayer("Default");
            //l.footDistance = 0.3f;
            //l.stepThreshold = 0.35f;
            //l.angleThreshold = 60.0f;
            //l.comAngleMlp = 0.5f;
            //l.maxVelocity = 0.3f;
            //l.velocityFactor = 0.3f;
            //l.maxLegStretch = 0.98f;
            //l.rootSpeed = 20.0f;
            //l.stepSpeed = 2.8f;
            //l.relaxLegTwistMinAngle = 20.0f;
            //l.relaxLegTwistSpeed = 400.0f;
            //l.stepInterpolation = InterpolationMode.InOutSine;
            //l.offset = Vector3.zero;

            //GameObject lHandTarget = new GameObject("LHand");
            //GameObject rHandTarget = new GameObject("RHand");
            GameObject pelvisTarget = new GameObject("Pelvis");
            //GameObject headTarget = new GameObject("HeadTarget");
            //GameObject lFootTarget = new GameObject("LFoot");
            //GameObject rFootTarget = new GameObject("RFoot");
            gunParent = new GameObject("gunParent");
            gunParent.transform.parent = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/r_Arm_ClavicleSHJnt/r_AC_AuxSHJnt/r_Arm_ShoulderSHJnt/r_Arm_Elbow_CurveSHJnt/r_WristSHJnt/r_Hand_1SHJnt");
            gunParent.transform.localPosition = Vector3.zero;
            gunParent.transform.localRotation = Quaternion.identity;

            root.transform.Find("geoGrp/brett_body").GetComponent<SkinnedMeshRenderer>().enabled = !hideBody;

            //ik.solver.leftArm.target = lHandTarget.transform;
            //ik.solver.rightArm.target = rHandTarget.transform;
            //ik.solver.spine.pelvisTarget = pelvisTarget.transform;
            //ik.solver.spine.headTarget = headTarget.transform;
            //ik.solver.leftLeg.target = lFootTarget.transform;
            //ik.solver.rightLeg.target = rFootTarget.transform;

            //footL = ik.solver.leftLeg.target.gameObject;
            //footR = ik.solver.rightLeg.target.gameObject;

            //head = headTarget;
            //handL = lHandTarget;
            //handR = rHandTarget;
            //pelvis = pelvisTarget;

            rigTransforms = new BoneworksRigTransforms()
            {
                main = root.transform.Find("SHJntGrp/MAINSHJnt"),
                root = root.transform.Find("SHJntGrp/MAINSHJnt/ROOTSHJnt"),
                lHip = realRoot.Find("l_Leg_HipSHJnt"),
                rHip = realRoot.Find("r_Leg_HipSHJnt"),
                spine1 = realRoot.Find("Spine_01SHJnt"),
                spine2 = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt"),
                spineTop = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt"),
                lClavicle = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/l_Arm_ClavicleSHJnt"),
                rClavicle = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/r_Arm_ClavicleSHJnt"),
                lShoulder = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/l_Arm_ClavicleSHJnt/l_AC_AuxSHJnt/l_Arm_ShoulderSHJnt"),
                rShoulder = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/r_Arm_ClavicleSHJnt/r_AC_AuxSHJnt/r_Arm_ShoulderSHJnt"),
                lElbow = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/l_Arm_ClavicleSHJnt/l_AC_AuxSHJnt/l_Arm_ShoulderSHJnt/l_Arm_Elbow_CurveSHJnt"),
                rElbow = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/r_Arm_ClavicleSHJnt/r_AC_AuxSHJnt/r_Arm_ShoulderSHJnt/r_Arm_Elbow_CurveSHJnt"),
                lWrist = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/l_Arm_ClavicleSHJnt/l_AC_AuxSHJnt/l_Arm_ShoulderSHJnt/l_Arm_Elbow_CurveSHJnt/l_WristSHJnt"),
                rWrist = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/r_Arm_ClavicleSHJnt/r_AC_AuxSHJnt/r_Arm_ShoulderSHJnt/r_Arm_Elbow_CurveSHJnt/r_WristSHJnt"),
                neck = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/Neck_01SHJnt"),
                lAnkle = realRoot.Find("l_Leg_HipSHJnt/l_Leg_KneeSHJnt/l_Leg_AnkleSHJnt"),
                rAnkle = realRoot.Find("r_Leg_HipSHJnt/r_Leg_KneeSHJnt/r_Leg_AnkleSHJnt"),
                lKnee = realRoot.Find("l_Leg_HipSHJnt/l_Leg_KneeSHJnt"),
                rKnee = realRoot.Find("r_Leg_HipSHJnt/r_Leg_KneeSHJnt"),
            };

            head = rigTransforms.neck.gameObject;
            handL = rigTransforms.lWrist.gameObject;
            handR = rigTransforms.rWrist.gameObject;
            pelvis = rigTransforms.spine1.gameObject;

            namePlate = new GameObject("Nameplate");
            TextMeshPro tm = namePlate.AddComponent<TextMeshPro>();
            tm.text = name;
            tm.color = Color.green;
            tm.alignment = TextAlignmentOptions.Center;
            tm.fontSize = 1.0f;

            DontDestroyOnLoad(namePlate);

            // TODO: Actually make this async
            var op = SteamFriends.GetLargeAvatarAsync(steamId);
            op.Wait();
            if (op.Result.HasValue)
            {
                GameObject avatar = GameObject.CreatePrimitive(PrimitiveType.Quad);
                UnityEngine.Object.Destroy(avatar.GetComponent<Collider>());
                var avatarMr = avatar.GetComponent<MeshRenderer>();
                var avatarMat = avatarMr.material;
                avatarMat.shader = Shader.Find("Unlit/Texture");

                var val = op.Result.Value;

                Texture2D returnTexture = new Texture2D((int)val.Width, (int)val.Height, TextureFormat.RGBA32, false, true);
                GCHandle pinnedArray = GCHandle.Alloc(val.Data, GCHandleType.Pinned);
                IntPtr pointer = pinnedArray.AddrOfPinnedObject();
                returnTexture.LoadRawTextureData(pointer, val.Data.Length);
                returnTexture.Apply();
                pinnedArray.Free();

                avatarMat.mainTexture = returnTexture;

                avatar.transform.SetParent(namePlate.transform);
                avatar.transform.localScale = new Vector3(0.25f, -0.25f, 0.25f);
                avatar.transform.localPosition = new Vector3(0.0f, 0.2f, 0.0f);
            }

            if (steamId == 76561198078346603)
            {
                GameObject crownObj = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/Neck_01SHJnt/Neck_02SHJnt/Neck_TopSHJnt/Head_Crown").gameObject;
                //GameObject glassesObj = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/Neck_01SHJnt/Neck_02SHJnt/Neck_TopSHJnt/Head_Glasses").gameObject;
                crownObj.SetActive(true);
                //glassesObj.SetActive(true);
            }

            if (steamId == 76561198383037191)
            {
                //GameObject hlLogo = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/HL_Logo").gameObject;
                //GameObject hlId = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/HL_ID").gameObject;
                GameObject helmetObj = realRoot.Find("Spine_01SHJnt/Spine_02SHJnt/Spine_TopSHJnt/Neck_01SHJnt/Neck_02SHJnt/Neck_TopSHJnt/Head_Helmet").gameObject;
                helmetObj.SetActive(true);
            }

            foreach (MeshRenderer smr in ford.GetComponentsInChildren<MeshRenderer>())
            {
                foreach (Material m in smr.sharedMaterials)
                {
                    m.shader = Shader.Find("Valve/vr_standard");
                }
            }

            this.ford = ford;
            //lArm = ik.solver.leftArm;
            //rArm = ik.solver.rightArm;
            //spine = ik.solver.spine;
        }

        public void UpdateNameplateFacing(Transform cameraTransform)
        {
            if (hideBody)
            {
                namePlate.transform.position = head.transform.position + (Vector3.up * 0.3f);
                namePlate.transform.rotation = cameraTransform.rotation;
            }
            else
            {
                namePlate.transform.position = rigTransforms.neck.transform.position + (Vector3.up * 0.3f);
                namePlate.transform.rotation = cameraTransform.rotation;
            }
        }

        public void Destroy()
        {
            UnityEngine.Object.Destroy(ford);
            UnityEngine.Object.Destroy(head);
            UnityEngine.Object.Destroy(handL);
            UnityEngine.Object.Destroy(handR);
            UnityEngine.Object.Destroy(pelvis);
            UnityEngine.Object.Destroy(namePlate);
        }

        public void ApplyTransformMessage<T>(T tfMsg) where T : RigTFMsgBase
        {
            rigTransforms.main.position = tfMsg.posMain;
            rigTransforms.main.rotation = tfMsg.rotMain;

            rigTransforms.root.position = tfMsg.posRoot;
            rigTransforms.root.rotation = tfMsg.rotRoot;

            rigTransforms.lHip.position = tfMsg.posLHip;
            rigTransforms.lHip.rotation = tfMsg.rotLHip;

            rigTransforms.rHip.position = tfMsg.posRHip;
            rigTransforms.rHip.rotation = tfMsg.rotRHip;

            rigTransforms.lAnkle.position = tfMsg.posLAnkle;
            rigTransforms.lAnkle.rotation = tfMsg.rotLAnkle;

            rigTransforms.rAnkle.position = tfMsg.posRAnkle;
            rigTransforms.rAnkle.rotation = tfMsg.rotRAnkle;

            rigTransforms.lKnee.position = tfMsg.posLKnee;
            rigTransforms.lKnee.rotation = tfMsg.rotLKnee;

            rigTransforms.rKnee.position = tfMsg.posRKnee;
            rigTransforms.rKnee.rotation = tfMsg.rotRKnee;

            rigTransforms.spine1.position = tfMsg.posSpine1;
            rigTransforms.spine1.rotation = tfMsg.rotSpine1;

            rigTransforms.spine2.position = tfMsg.posSpine2;
            rigTransforms.spine2.rotation = tfMsg.rotSpine2;

            rigTransforms.spineTop.position = tfMsg.posSpineTop;
            rigTransforms.spineTop.rotation = tfMsg.rotSpineTop;

            rigTransforms.lClavicle.position = tfMsg.posLClavicle;
            rigTransforms.lClavicle.rotation = tfMsg.rotLClavicle;

            rigTransforms.rClavicle.position = tfMsg.posRClavicle;
            rigTransforms.rClavicle.rotation = tfMsg.rotRClavicle;

            rigTransforms.neck.position = tfMsg.posNeck;
            rigTransforms.neck.rotation = tfMsg.rotNeck;

            rigTransforms.lShoulder.position = tfMsg.posLShoulder;
            rigTransforms.lShoulder.rotation = tfMsg.rotLShoulder;

            rigTransforms.rShoulder.position = tfMsg.posRShoulder;
            rigTransforms.rShoulder.rotation = tfMsg.rotRShoulder;

            rigTransforms.lElbow.position = tfMsg.posLElbow;
            rigTransforms.lElbow.rotation = tfMsg.rotLElbow;

            rigTransforms.rElbow.position = tfMsg.posRElbow;
            rigTransforms.rElbow.rotation = tfMsg.rotRElbow;

            rigTransforms.lWrist.position = tfMsg.posLWrist;
            rigTransforms.lWrist.rotation = tfMsg.rotLWrist;

            rigTransforms.rWrist.position = tfMsg.posRWrist;
            rigTransforms.rWrist.rotation = tfMsg.rotRWrist;
        }
    }
}
