using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

//孫オブジェクトまで取得する https://kazuooooo.hatenablog.com/entry/2015/08/07/010938

public class ComponentMigrationTool : EditorWindow
{
	[MenuItem("Window/ComponentMigrationTool")]
    public static void ShowWindow()
	{
		EditorWindow.GetWindow<ComponentMigrationTool>();
	}

	//AllComponents
	private bool _allComponents = false; //有効化・無効化

	//Animator
	private bool _animator = false; //有効化・無効化
	private bool _allAnimator = false; //全て選択
	//個別選択
	public struct CMT_Animator
	{
		public bool controller;
		public bool avatar;
		public bool applyRootMotion;
		public bool updateMode;
		public bool cullingMode;
	}

	//SkinnedMeshRenderer
	private bool _skinnedMeshRenderer = false; //有効化・無効化
	private bool _allSkinnedMeshRenderer = false; //全て選択
	//個別選択
	public struct CMT_SkinnedMeshRenderer
	{
		public bool bounds;
		public bool quality;
		public bool updateWhenOffscreen;
		public bool rootBone;
		public bool mesh;
		public bool material;
		public bool castShadows;
		public bool receiveShadows;
		public bool lightProbes;
		public bool reflectionProbes;
		public bool anchorOverride;
		public bool motionVectors;
		public bool dynamicOcdusion;
	}

	//MeshRenderer
	private bool _meshRenderer = false; //有効化・無効化
	private bool _allMeshRenderer = false; //全て選択
	//個別選択
	public struct CMT_MeshRenderer
	{
		public bool material;
		public bool castShadows;
		public bool receiveShadows;
		public bool contributeGlobalIllumination;
		public bool receiveGlobalIllumination;
		public bool lightProbes;
		public bool reflectionProbes;
		public bool anchorOverride;
		public bool motionVectors;
		public bool dynamicOcdusion;
	}

	//オブジェクト
	private GameObject _oldObject;
	private GameObject _newObject;

	//Foldout
	private bool _foldoutAnimator = false;
	private bool _animatorAdvancedSettings = false;
	private bool _foldoutSkinnedMeshRenderer = false;
	private bool _skinnedMeshRendererAdvancedSettings = false;
	private bool _foldoutMeshRenderer = false;
	private bool _meshRendererAdvancedSettings = false;

	//構造体インスタンス化
	private CMT_Animator _cmtAnimator;
	private CMT_SkinnedMeshRenderer _cmtSkinnedMeshRenderer;
	private CMT_MeshRenderer _cmtMeshRenderer;

	//画面いっぱいに出た時スクロールできるようにする
	private Vector2 _scroll = Vector2.zero;

	//ボーンを変更するかチェックする
	private bool _boneCheck = false;

	private bool _anchorOverride = false;

    private bool _meshCheck = false;

	//言語変更
	private enum Language
	{
		English,
		Japanese,
	}

	//Languageのインスタンス化
	private Language _language;

	private void OnGUI()
	{
		//英語
		if (_language.ToString() == "English")
		{
			_language = (Language)EditorGUILayout.EnumPopup("Language", _language);

			//スクロールスタート
			_scroll = EditorGUILayout.BeginScrollView(_scroll);

			_oldObject = EditorGUILayout.ObjectField("Old Object", _oldObject, typeof(GameObject), true) as GameObject;
			_newObject = EditorGUILayout.ObjectField("New Object", _newObject, typeof(GameObject), true) as GameObject;

			EditorGUILayout.Space(18);

			if (_oldObject && _newObject)
			{
				_allComponents = EditorGUILayout.ToggleLeft("All Components", _allComponents);

				EditorGUILayout.Space(6);

				if (!_allComponents)
				{
					//Animator
					EditorGUILayout.BeginVertical("Box");
					_foldoutAnimator = EditorGUILayout.Foldout(_foldoutAnimator, "Animator");
					if (_foldoutAnimator)
					{
						_animator = EditorGUILayout.ToggleLeft("Activate Animator", _animator);
						if (_animator)
						{
							if (_cmtAnimator.controller &&
								_cmtAnimator.avatar &&
								_cmtAnimator.applyRootMotion &&
								_cmtAnimator.updateMode &&
								_cmtAnimator.cullingMode)
							{
								_allAnimator = false;
							}
							else
							{
								_allAnimator = EditorGUILayout.ToggleLeft("All Animator", _allAnimator);
							}

							if (!_allAnimator)
							{
								_animatorAdvancedSettings = EditorGUILayout.Foldout(_animatorAdvancedSettings, "Advanced Settings");
								if (_animatorAdvancedSettings)
								{
									_cmtAnimator.controller = EditorGUILayout.ToggleLeft("Controller", _cmtAnimator.controller);
									_cmtAnimator.avatar = EditorGUILayout.ToggleLeft("Avatar", _cmtAnimator.avatar);
									_cmtAnimator.applyRootMotion = EditorGUILayout.ToggleLeft("Apply Root Motion", _cmtAnimator.applyRootMotion);
									_cmtAnimator.updateMode = EditorGUILayout.ToggleLeft("Update Mode", _cmtAnimator.updateMode);
									_cmtAnimator.cullingMode = EditorGUILayout.ToggleLeft("Culling Mode", _cmtAnimator.cullingMode);
								}
							}
							else
							{
								_cmtAnimator.controller = false;
								_cmtAnimator.avatar = false;
								_cmtAnimator.applyRootMotion = false;
								_cmtAnimator.updateMode = false;
								_cmtAnimator.cullingMode = false;
							}
						}
					}
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space(6);

					EditorGUILayout.BeginVertical("Box");
					//SkinnedMeshRenderer
					_foldoutSkinnedMeshRenderer = EditorGUILayout.Foldout(_foldoutSkinnedMeshRenderer, "Skinned Mesh Renderer");
					if (_foldoutSkinnedMeshRenderer)
					{
						_skinnedMeshRenderer = EditorGUILayout.ToggleLeft("Activate Skinned Mesh Renderer", _skinnedMeshRenderer);
						if (_skinnedMeshRenderer)
						{
							if (_cmtSkinnedMeshRenderer.bounds &&
								_cmtSkinnedMeshRenderer.quality &&
								_cmtSkinnedMeshRenderer.updateWhenOffscreen &&
								_cmtSkinnedMeshRenderer.rootBone &&
								_cmtSkinnedMeshRenderer.mesh &&
								_cmtSkinnedMeshRenderer.material &&
								_cmtSkinnedMeshRenderer.castShadows &&
								_cmtSkinnedMeshRenderer.receiveShadows &&
								_cmtSkinnedMeshRenderer.lightProbes &&
								_cmtSkinnedMeshRenderer.reflectionProbes &&
								_cmtSkinnedMeshRenderer.anchorOverride &&
								_cmtSkinnedMeshRenderer.motionVectors &&
								_cmtSkinnedMeshRenderer.dynamicOcdusion)
							{
								_allSkinnedMeshRenderer = false;
							}
							else
							{
								_allSkinnedMeshRenderer = EditorGUILayout.ToggleLeft("All Skinned Mesh Renderer", _allSkinnedMeshRenderer);
							}

							if (!_allSkinnedMeshRenderer)
							{
								_skinnedMeshRendererAdvancedSettings = EditorGUILayout.Foldout(_skinnedMeshRendererAdvancedSettings, "Advanced Settings");
								if (_skinnedMeshRendererAdvancedSettings)
								{
									_cmtSkinnedMeshRenderer.bounds = EditorGUILayout.ToggleLeft("Bounds", _cmtSkinnedMeshRenderer.bounds);
									_cmtSkinnedMeshRenderer.quality = EditorGUILayout.ToggleLeft("Quality", _cmtSkinnedMeshRenderer.quality);
									_cmtSkinnedMeshRenderer.updateWhenOffscreen = EditorGUILayout.ToggleLeft("Update When Offscreen", _cmtSkinnedMeshRenderer.updateWhenOffscreen);
									_cmtSkinnedMeshRenderer.rootBone = EditorGUILayout.ToggleLeft("Root Bone", _cmtSkinnedMeshRenderer.rootBone);
									_cmtSkinnedMeshRenderer.mesh = EditorGUILayout.ToggleLeft("Mesh", _cmtSkinnedMeshRenderer.mesh);
									_cmtSkinnedMeshRenderer.material = EditorGUILayout.ToggleLeft("Material", _cmtSkinnedMeshRenderer.material);
									_cmtSkinnedMeshRenderer.castShadows = EditorGUILayout.ToggleLeft("Cast Shadows", _cmtSkinnedMeshRenderer.castShadows);
									_cmtSkinnedMeshRenderer.receiveShadows = EditorGUILayout.ToggleLeft("Receive Shadows", _cmtSkinnedMeshRenderer.receiveShadows);
									_cmtSkinnedMeshRenderer.lightProbes = EditorGUILayout.ToggleLeft("Light Probes", _cmtSkinnedMeshRenderer.lightProbes);
									_cmtSkinnedMeshRenderer.reflectionProbes = EditorGUILayout.ToggleLeft("Reflection Probes", _cmtSkinnedMeshRenderer.reflectionProbes);
									_cmtSkinnedMeshRenderer.anchorOverride = EditorGUILayout.ToggleLeft("Anchor Override", _cmtSkinnedMeshRenderer.anchorOverride);
									_cmtSkinnedMeshRenderer.motionVectors = EditorGUILayout.ToggleLeft("Motion Vectors", _cmtSkinnedMeshRenderer.motionVectors);
									_cmtSkinnedMeshRenderer.dynamicOcdusion = EditorGUILayout.ToggleLeft("Dynamic Ocdusion", _cmtSkinnedMeshRenderer.dynamicOcdusion);
								}
							}
							else
							{
								_cmtSkinnedMeshRenderer.bounds = false;
								_cmtSkinnedMeshRenderer.quality = false;
								_cmtSkinnedMeshRenderer.updateWhenOffscreen = false;
								_cmtSkinnedMeshRenderer.rootBone = false;
								_cmtSkinnedMeshRenderer.mesh = false;
								_cmtSkinnedMeshRenderer.material = false;
								_cmtSkinnedMeshRenderer.castShadows = false;
								_cmtSkinnedMeshRenderer.receiveShadows = false;
								_cmtSkinnedMeshRenderer.lightProbes = false;
								_cmtSkinnedMeshRenderer.reflectionProbes = false;
								_cmtSkinnedMeshRenderer.anchorOverride = false;
								_cmtSkinnedMeshRenderer.motionVectors = false;
								_cmtSkinnedMeshRenderer.dynamicOcdusion = false;
							}
						}
					}
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space(6);

					EditorGUILayout.BeginVertical("Box");
					//MeshRenderer
					_foldoutMeshRenderer = EditorGUILayout.Foldout(_foldoutMeshRenderer, "Mesh Renderer");
					if (_foldoutMeshRenderer)
					{
						_meshRenderer = EditorGUILayout.ToggleLeft("Activate Mesh Renderer", _meshRenderer);
						if (_meshRenderer)
						{
							if (_cmtMeshRenderer.material &&
								_cmtMeshRenderer.castShadows &&
								_cmtMeshRenderer.receiveShadows &&
								_cmtMeshRenderer.contributeGlobalIllumination &&
								_cmtMeshRenderer.receiveGlobalIllumination &&
								_cmtMeshRenderer.lightProbes &&
								_cmtMeshRenderer.reflectionProbes &&
								_cmtMeshRenderer.anchorOverride &&
								_cmtMeshRenderer.motionVectors &&
								_cmtMeshRenderer.dynamicOcdusion)
							{
								_allMeshRenderer = false;
							}
							else
							{
								_allMeshRenderer = EditorGUILayout.ToggleLeft("All Mesh Renderer", _allMeshRenderer);
							}

							if (!_allMeshRenderer)
							{
								_meshRendererAdvancedSettings = EditorGUILayout.Foldout(_meshRendererAdvancedSettings, "Advanced Settings");
								if (_meshRendererAdvancedSettings)
								{
									_cmtMeshRenderer.material = EditorGUILayout.ToggleLeft("Material", _cmtMeshRenderer.material);
									_cmtMeshRenderer.castShadows = EditorGUILayout.ToggleLeft("Cast Shadows", _cmtMeshRenderer.castShadows);
									_cmtMeshRenderer.receiveShadows = EditorGUILayout.ToggleLeft("Receive Shadows", _cmtMeshRenderer.receiveShadows);
									_cmtMeshRenderer.contributeGlobalIllumination = EditorGUILayout.ToggleLeft("Contribute Global Illumination", _cmtMeshRenderer.contributeGlobalIllumination);
									_cmtMeshRenderer.receiveGlobalIllumination = EditorGUILayout.ToggleLeft("Receive Global Illumination", _cmtMeshRenderer.receiveGlobalIllumination);
									_cmtMeshRenderer.lightProbes = EditorGUILayout.ToggleLeft("Light Probes", _cmtMeshRenderer.lightProbes);
									_cmtMeshRenderer.reflectionProbes = EditorGUILayout.ToggleLeft("Reflection Probes", _cmtMeshRenderer.reflectionProbes);
									_cmtMeshRenderer.anchorOverride = EditorGUILayout.ToggleLeft("Anchor Override", _cmtMeshRenderer.anchorOverride);
									_cmtMeshRenderer.motionVectors = EditorGUILayout.ToggleLeft("Motion Vectors", _cmtMeshRenderer.motionVectors);
									_cmtMeshRenderer.dynamicOcdusion = EditorGUILayout.ToggleLeft("Dynamic Ocdusion", _cmtMeshRenderer.dynamicOcdusion);
								}
							}
							else
							{
								_cmtMeshRenderer.material = false;
								_cmtMeshRenderer.castShadows = false;
								_cmtMeshRenderer.receiveShadows = false;
								_cmtMeshRenderer.contributeGlobalIllumination = false;
								_cmtMeshRenderer.receiveGlobalIllumination = false;
								_cmtMeshRenderer.lightProbes = false;
								_cmtMeshRenderer.reflectionProbes = false;
								_cmtMeshRenderer.anchorOverride = false;
								_cmtMeshRenderer.motionVectors = false;
								_cmtMeshRenderer.dynamicOcdusion = false;
							}
						}
					}
					EditorGUILayout.EndVertical();
				}
				else
				{
					_animator = false;
					_allAnimator = false;
					_cmtAnimator.controller = false;
					_cmtAnimator.avatar = false;
					_cmtAnimator.applyRootMotion = false;
					_cmtAnimator.updateMode = false;
					_cmtAnimator.cullingMode = false;
					_skinnedMeshRenderer = false;
					_allSkinnedMeshRenderer = false;
					_cmtSkinnedMeshRenderer.bounds = false;
					_cmtSkinnedMeshRenderer.quality = false;
					_cmtSkinnedMeshRenderer.updateWhenOffscreen = false;
					_cmtSkinnedMeshRenderer.rootBone = false;
					_cmtSkinnedMeshRenderer.mesh = false;
					_cmtSkinnedMeshRenderer.material = false;
					_cmtSkinnedMeshRenderer.lightProbes = false;
					_cmtSkinnedMeshRenderer.reflectionProbes = false;
					_cmtSkinnedMeshRenderer.anchorOverride = false;
					_cmtSkinnedMeshRenderer.motionVectors = false;
					_cmtSkinnedMeshRenderer.dynamicOcdusion = false;
					_meshRenderer = false;
					_allMeshRenderer = false;
					_cmtMeshRenderer.material = false;
					_cmtMeshRenderer.castShadows = false;
					_cmtMeshRenderer.receiveShadows = false;
					_cmtMeshRenderer.contributeGlobalIllumination = false;
					_cmtMeshRenderer.receiveGlobalIllumination = false;
					_cmtMeshRenderer.lightProbes = false;
					_cmtMeshRenderer.reflectionProbes = false;
					_cmtMeshRenderer.anchorOverride = false;
					_cmtMeshRenderer.motionVectors = false;
					_cmtMeshRenderer.dynamicOcdusion = false;
				}
			}

			GUILayout.Space(6);
			if(_animator || _skinnedMeshRenderer || _meshRenderer || _allComponents)
			{
				if(_oldObject && _newObject)
				{
					if (GUILayout.Button("Execution"))
					{
						if (_allComponents)
						{
							AllComponents();
						}
						else
						{
							if (_animator)
							{
								AnimaterComponent();
							}
							if (_skinnedMeshRenderer)
							{
								SkinnedMeshRenderer();
							}
							if (_meshRenderer)
							{
								MeshRendererComponent();
							}
						}
						EditorUtility.DisplayDialog("Confirmation", "The process is complete", "OK");
					}
				}
			}
		}
		//日本語
		else if(_language.ToString() == "Japanese")
		{
			_language = (Language)EditorGUILayout.EnumPopup("言語", _language);

			//スクロールスタート
			_scroll = EditorGUILayout.BeginScrollView(_scroll);

			_oldObject = EditorGUILayout.ObjectField("旧 オブジェクト", _oldObject, typeof(GameObject), true) as GameObject;
			_newObject = EditorGUILayout.ObjectField("新 オブジェクト", _newObject, typeof(GameObject), true) as GameObject;

			EditorGUILayout.Space(18);

			if (_oldObject && _newObject)
			{
				_allComponents = EditorGUILayout.ToggleLeft("全てのコンポーネント", _allComponents);

				EditorGUILayout.Space(6);

				if (!_allComponents)
				{
					//Animator
					EditorGUILayout.BeginVertical("Box");
					_foldoutAnimator = EditorGUILayout.Foldout(_foldoutAnimator, "アニメーター");
					if (_foldoutAnimator)
					{
						_animator = EditorGUILayout.ToggleLeft("アニメーターを有効化", _animator);
						if (_animator)
						{
							if (_cmtAnimator.controller &&
								_cmtAnimator.avatar &&
								_cmtAnimator.applyRootMotion &&
								_cmtAnimator.updateMode &&
								_cmtAnimator.cullingMode)
							{
								_allAnimator = false;
							}
							else
							{
								_allAnimator = EditorGUILayout.ToggleLeft("全てのアニメーター", _allAnimator);
							}

							if (!_allAnimator)
							{
								_animatorAdvancedSettings = EditorGUILayout.Foldout(_animatorAdvancedSettings, "詳細設定");
								if (_animatorAdvancedSettings)
								{
									_cmtAnimator.controller = EditorGUILayout.ToggleLeft("コントローラー", _cmtAnimator.controller);
									_cmtAnimator.avatar = EditorGUILayout.ToggleLeft("アバター", _cmtAnimator.avatar);
									_cmtAnimator.applyRootMotion = EditorGUILayout.ToggleLeft("ルートモーションを適用", _cmtAnimator.applyRootMotion);
									_cmtAnimator.updateMode = EditorGUILayout.ToggleLeft("更新モード", _cmtAnimator.updateMode);
									_cmtAnimator.cullingMode = EditorGUILayout.ToggleLeft("カリングモード", _cmtAnimator.cullingMode);
								}
							}
							else
							{
								_cmtAnimator.controller = false;
								_cmtAnimator.avatar = false;
								_cmtAnimator.applyRootMotion = false;
								_cmtAnimator.updateMode = false;
								_cmtAnimator.cullingMode = false;
							}
						}
					}
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space(6);

					EditorGUILayout.BeginVertical("Box");
					//SkinnedMeshRenderer
					_foldoutSkinnedMeshRenderer = EditorGUILayout.Foldout(_foldoutSkinnedMeshRenderer, "スキンメッシュレンダラー");
					if (_foldoutSkinnedMeshRenderer)
					{
						_skinnedMeshRenderer = EditorGUILayout.ToggleLeft("スキンメッシュレンダラーを有効化", _skinnedMeshRenderer);
						if (_skinnedMeshRenderer)
						{
							if (_cmtSkinnedMeshRenderer.bounds &&
								_cmtSkinnedMeshRenderer.quality &&
								_cmtSkinnedMeshRenderer.updateWhenOffscreen &&
								_cmtSkinnedMeshRenderer.rootBone &&
								_cmtSkinnedMeshRenderer.mesh &&
								_cmtSkinnedMeshRenderer.material &&
								_cmtSkinnedMeshRenderer.castShadows &&
								_cmtSkinnedMeshRenderer.receiveShadows &&
								_cmtSkinnedMeshRenderer.lightProbes &&
								_cmtSkinnedMeshRenderer.reflectionProbes &&
								_cmtSkinnedMeshRenderer.anchorOverride &&
								_cmtSkinnedMeshRenderer.motionVectors &&
								_cmtSkinnedMeshRenderer.dynamicOcdusion)
							{
								_allSkinnedMeshRenderer = false;
							}
							else
							{
								_allSkinnedMeshRenderer = EditorGUILayout.ToggleLeft("全てのスキンメッシュレンダラー", _allSkinnedMeshRenderer);
							}

							if (!_allSkinnedMeshRenderer)
							{
								_skinnedMeshRendererAdvancedSettings = EditorGUILayout.Foldout(_skinnedMeshRendererAdvancedSettings, "詳細設定");
								if (_skinnedMeshRendererAdvancedSettings)
								{
									_cmtSkinnedMeshRenderer.bounds = EditorGUILayout.ToggleLeft("範囲", _cmtSkinnedMeshRenderer.bounds);
									_cmtSkinnedMeshRenderer.quality = EditorGUILayout.ToggleLeft("品質", _cmtSkinnedMeshRenderer.quality);
									_cmtSkinnedMeshRenderer.updateWhenOffscreen = EditorGUILayout.ToggleLeft("オフスクリーン時に更新", _cmtSkinnedMeshRenderer.updateWhenOffscreen);
									_cmtSkinnedMeshRenderer.rootBone = EditorGUILayout.ToggleLeft("ルートボーン", _cmtSkinnedMeshRenderer.rootBone);
									_cmtSkinnedMeshRenderer.mesh = EditorGUILayout.ToggleLeft("メッシュ", _cmtSkinnedMeshRenderer.mesh);
									_cmtSkinnedMeshRenderer.material = EditorGUILayout.ToggleLeft("マテリアル", _cmtSkinnedMeshRenderer.material);
									_cmtMeshRenderer.lightProbes = EditorGUILayout.ToggleLeft("投影", _cmtMeshRenderer.lightProbes);
									_cmtMeshRenderer.reflectionProbes = EditorGUILayout.ToggleLeft("影を受ける", _cmtMeshRenderer.reflectionProbes);
									_cmtSkinnedMeshRenderer.lightProbes = EditorGUILayout.ToggleLeft("ライトブローブ", _cmtSkinnedMeshRenderer.lightProbes);
									_cmtSkinnedMeshRenderer.reflectionProbes = EditorGUILayout.ToggleLeft("リフレクションブローブ", _cmtSkinnedMeshRenderer.reflectionProbes);
									_cmtSkinnedMeshRenderer.anchorOverride = EditorGUILayout.ToggleLeft("アンカーオーバーライド", _cmtSkinnedMeshRenderer.anchorOverride);
									_cmtSkinnedMeshRenderer.motionVectors = EditorGUILayout.ToggleLeft("スキンしたモーションベクトル", _cmtSkinnedMeshRenderer.motionVectors);
									_cmtSkinnedMeshRenderer.dynamicOcdusion = EditorGUILayout.ToggleLeft("動的オクルージョン", _cmtSkinnedMeshRenderer.dynamicOcdusion);
								}
							}
							else
							{
								_cmtSkinnedMeshRenderer.bounds = false;
								_cmtSkinnedMeshRenderer.quality = false;
								_cmtSkinnedMeshRenderer.updateWhenOffscreen = false;
								_cmtSkinnedMeshRenderer.rootBone = false;
								_cmtSkinnedMeshRenderer.mesh = false;
								_cmtSkinnedMeshRenderer.material = false;
								_cmtSkinnedMeshRenderer.castShadows = false;
								_cmtSkinnedMeshRenderer.receiveShadows = false;
								_cmtSkinnedMeshRenderer.lightProbes = false;
								_cmtSkinnedMeshRenderer.reflectionProbes = false;
								_cmtSkinnedMeshRenderer.anchorOverride = false;
								_cmtSkinnedMeshRenderer.motionVectors = false;
								_cmtSkinnedMeshRenderer.dynamicOcdusion = false;
							}
						}
					}
					EditorGUILayout.EndVertical();

					EditorGUILayout.Space(6);

					EditorGUILayout.BeginVertical("Box");
					//MeshRenderer
					_foldoutMeshRenderer = EditorGUILayout.Foldout(_foldoutMeshRenderer, "メッシュレンダラー");
					if (_foldoutMeshRenderer)
					{
						_meshRenderer = EditorGUILayout.ToggleLeft("メッシュレンダラーを有効化", _meshRenderer);
						if (_meshRenderer)
						{
							if (_cmtMeshRenderer.material &&
								_cmtMeshRenderer.castShadows &&
								_cmtMeshRenderer.receiveShadows &&
								_cmtMeshRenderer.contributeGlobalIllumination &&
								_cmtMeshRenderer.receiveGlobalIllumination &&
								_cmtMeshRenderer.lightProbes &&
								_cmtMeshRenderer.reflectionProbes &&
								_cmtMeshRenderer.anchorOverride &&
								_cmtMeshRenderer.motionVectors &&
								_cmtMeshRenderer.dynamicOcdusion)
							{
								_allMeshRenderer = false;
							}
							else
							{
								_allMeshRenderer = EditorGUILayout.ToggleLeft("全てのメッシュレンダラー", _allMeshRenderer);
							}

							if (!_allMeshRenderer)
							{
								_meshRendererAdvancedSettings = EditorGUILayout.Foldout(_meshRendererAdvancedSettings, "詳細設定");
								if (_meshRendererAdvancedSettings)
								{
									_cmtMeshRenderer.material = EditorGUILayout.ToggleLeft("マテリアル", _cmtMeshRenderer.material);
									_cmtMeshRenderer.castShadows = EditorGUILayout.ToggleLeft("投影", _cmtMeshRenderer.castShadows);
									_cmtMeshRenderer.receiveShadows = EditorGUILayout.ToggleLeft("影を受ける", _cmtMeshRenderer.receiveShadows);
									_cmtMeshRenderer.contributeGlobalIllumination = EditorGUILayout.ToggleLeft("グローバルイルミネーションに影響", _cmtMeshRenderer.contributeGlobalIllumination);
									_cmtMeshRenderer.receiveGlobalIllumination = EditorGUILayout.ToggleLeft("グローバルイルミネーションを受ける", _cmtMeshRenderer.receiveGlobalIllumination);
									_cmtMeshRenderer.lightProbes = EditorGUILayout.ToggleLeft("ライトブローブ", _cmtMeshRenderer.lightProbes);
									_cmtMeshRenderer.reflectionProbes = EditorGUILayout.ToggleLeft("リフレクションブローブ", _cmtMeshRenderer.reflectionProbes);
									_cmtMeshRenderer.anchorOverride = EditorGUILayout.ToggleLeft("アンカーオーバーライド", _cmtMeshRenderer.anchorOverride);
									_cmtMeshRenderer.motionVectors = EditorGUILayout.ToggleLeft("モーションベクトル", _cmtMeshRenderer.motionVectors);
									_cmtMeshRenderer.dynamicOcdusion = EditorGUILayout.ToggleLeft("動的オクルージョン", _cmtMeshRenderer.dynamicOcdusion);
								}
							}
							else
							{
								_cmtMeshRenderer.material = false;
								_cmtMeshRenderer.castShadows = false;
								_cmtMeshRenderer.receiveShadows = false;
								_cmtMeshRenderer.contributeGlobalIllumination = false;
								_cmtMeshRenderer.receiveGlobalIllumination = false;
								_cmtMeshRenderer.lightProbes = false;
								_cmtMeshRenderer.reflectionProbes = false;
								_cmtMeshRenderer.anchorOverride = false;
								_cmtMeshRenderer.motionVectors = false;
								_cmtMeshRenderer.dynamicOcdusion = false;
							}
						}
					}
					EditorGUILayout.EndVertical();
				}
				else
				{
					_animator = false;
					_allAnimator = false;
					_cmtAnimator.controller = false;
					_cmtAnimator.avatar = false;
					_cmtAnimator.applyRootMotion = false;
					_cmtAnimator.updateMode = false;
					_cmtAnimator.cullingMode = false;
					_skinnedMeshRenderer = false;
					_allSkinnedMeshRenderer = false;
					_cmtSkinnedMeshRenderer.bounds = false;
					_cmtSkinnedMeshRenderer.quality = false;
					_cmtSkinnedMeshRenderer.updateWhenOffscreen = false;
					_cmtSkinnedMeshRenderer.rootBone = false;
					_cmtSkinnedMeshRenderer.mesh = false;
					_cmtSkinnedMeshRenderer.material = false;
					_cmtSkinnedMeshRenderer.lightProbes = false;
					_cmtSkinnedMeshRenderer.reflectionProbes = false;
					_cmtSkinnedMeshRenderer.anchorOverride = false;
					_cmtSkinnedMeshRenderer.motionVectors = false;
					_cmtSkinnedMeshRenderer.dynamicOcdusion = false;
					_meshRenderer = false;
					_allMeshRenderer = false;
					_cmtMeshRenderer.material = false;
					_cmtMeshRenderer.castShadows = false;
					_cmtMeshRenderer.receiveShadows = false;
					_cmtMeshRenderer.contributeGlobalIllumination = false;
					_cmtMeshRenderer.receiveGlobalIllumination = false;
					_cmtMeshRenderer.lightProbes = false;
					_cmtMeshRenderer.reflectionProbes = false;
					_cmtMeshRenderer.anchorOverride = false;
					_cmtMeshRenderer.motionVectors = false;
					_cmtMeshRenderer.dynamicOcdusion = false;
				}
			}

			GUILayout.Space(6);
			if (_animator || _skinnedMeshRenderer || _meshRenderer || _allComponents)
			{
				if (_oldObject && _newObject)
				{
					if (GUILayout.Button("実行"))
					{
						if (_allComponents)
						{
							AllComponents();
						}
						else
						{
							if (_animator)
							{
								AnimaterComponent();
							}
							if (_skinnedMeshRenderer)
							{
								SkinnedMeshRenderer();
							}
							if (_meshRenderer)
							{
								MeshRendererComponent();
							}
						}
						EditorUtility.DisplayDialog("確認", "処理が終了しました", "はい");
					}
				}
			}
		}
		//スクロールエンド
		EditorGUILayout.EndScrollView();
	}

	//Hierarchyのパスを取得する
	private static string GetHierarchyPath(GameObject targetObj)
	{
		List<GameObject> objPath = new List<GameObject>();
		objPath.Add(targetObj);
		for (int i = 0; objPath[i].transform.parent != null; i++)
		{
			objPath.Add(objPath[i].transform.parent.gameObject);
		}
		string path = objPath[objPath.Count - 2].gameObject.name;
		for (int i = objPath.Count - 3; i >= 0; i--)
		{
			path += "/" + objPath[i].gameObject.name;
		}

		return path;
	}

	//全てのコンポーネントを移行する
	private void AllComponents()
	{
		//ボーンをコピーするか確認
		if (_language.ToString() == "English")
		{
			if (EditorUtility.DisplayDialog("Confirmation", "Do you want to copy the Bourne?", "OK", "NO"))
			{
				_boneCheck = true;
			}
			else
			{
				_boneCheck = false;
			}
		}
		else if(_language.ToString() == "Japanese")
		{
			if (EditorUtility.DisplayDialog("確認", "ボーンをコピーしますか？", "はい", "いいえ"))
			{
                _boneCheck = true;
			}
			else
			{
                _boneCheck = false;
			}
		}

        //メッシュをコピーするか確認
        if (_language.ToString() == "English")
        {
            if (EditorUtility.DisplayDialog("Confirmation", "Do you want to copy the Mesh?", "OK", "NO"))
            {
                _meshCheck = true;
            }
            else
            {
                _meshCheck = false;
            }
        }
        else if (_language.ToString() == "Japanese")
        {
            if (EditorUtility.DisplayDialog("確認", "メッシュをコピーしますか？", "はい", "いいえ"))
            {
                _meshCheck = true;
            }
            else
            {
                _meshCheck = false;
            }
        }

        //アンカーオーバーライドをコピーするか確認
        if (_language.ToString() == "English")
		{
			if (EditorUtility.DisplayDialog("Confirmation", "Do you want to copy the anchor override?", "OK", "NO"))
			{
                _anchorOverride = true;
			}
			else
			{
                _anchorOverride = false;
			}
		}
		else if (_language.ToString() == "Japanese")
		{
			if (EditorUtility.DisplayDialog("確認", "アンカーオーバーライドをコピーしますか？", "はい", "いいえ"))
			{
                _anchorOverride = true;
			}
			else
			{
                _anchorOverride = false;
			}
		}

		//親オブジェクトのコンポーネントを移行
		Component[] parentComponents = _oldObject.GetComponents<Component>();
		foreach (var component in parentComponents)
		{
			if (_oldObject.GetComponent(component.GetType()).GetType() != typeof(Transform))
			{
				UnityEditorInternal.ComponentUtility.CopyComponent(_oldObject.GetComponent(component.GetType()));
				if (_newObject.GetComponent(component.GetType()))
				{
					UnityEditorInternal.ComponentUtility.PasteComponentValues(_newObject.GetComponent(component.GetType()));
				}
				else
				{
					UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_newObject);
				}
			}
		}

		//子・孫オブジェクトのコンポーネントを移行
		GameObject[] gameObjects = GetAllChildren.GetAll(_oldObject);
		foreach (var gameObject in gameObjects)
		{
			string objectPath = GetHierarchyPath(gameObject);
			GameObject newObjectCheck = GameObject.Find(_newObject.name + "/" + objectPath)?.gameObject;
			if (newObjectCheck) {
				Component[] components = gameObject.GetComponents<Component>();
				foreach (var component in components)
				{
					Component oldComponent = gameObject.GetComponent(component.GetType());
					if (oldComponent.GetType() != typeof(Transform))
					{
						//スキンメッシュだけそのままコピーするとバグるため対策
						if (oldComponent.GetType() == typeof(SkinnedMeshRenderer))
						{
							if (!newObjectCheck.GetComponent<SkinnedMeshRenderer>())
							{
								newObjectCheck.AddComponent<SkinnedMeshRenderer>();
							}
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().localBounds = gameObject.GetComponent<SkinnedMeshRenderer>().localBounds;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().quality = gameObject.GetComponent<SkinnedMeshRenderer>().quality;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = gameObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen;
							if(_boneCheck) newObjectCheck.GetComponent<SkinnedMeshRenderer>().rootBone = gameObject.GetComponent<SkinnedMeshRenderer>().rootBone;
							if(_meshCheck) newObjectCheck.GetComponent<SkinnedMeshRenderer>().sharedMesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().sharedMaterials = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = gameObject.GetComponent<SkinnedMeshRenderer>().shadowCastingMode;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().receiveShadows = gameObject.GetComponent<SkinnedMeshRenderer>().receiveShadows;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().lightProbeUsage = gameObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage;
							if(gameObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage == UnityEngine.Rendering.LightProbeUsage.UseProxyVolume)
							{
								newObjectCheck.GetComponent<SkinnedMeshRenderer>().lightProbeProxyVolumeOverride = gameObject.GetComponent<SkinnedMeshRenderer>().lightProbeProxyVolumeOverride;
							}
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage = gameObject.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage;
							if(_anchorOverride) newObjectCheck.GetComponent<SkinnedMeshRenderer>().probeAnchor = gameObject.GetComponent<SkinnedMeshRenderer>().probeAnchor;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors = gameObject.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic = gameObject.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic;
						}
						else
						{
							UnityEditorInternal.ComponentUtility.CopyComponent(oldComponent);
							if (newObjectCheck.GetComponent(component.GetType()))
							{
								UnityEditorInternal.ComponentUtility.PasteComponentValues(newObjectCheck.GetComponent(component.GetType()));
							}
							else
							{
								UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newObjectCheck);
							}
						}
					}
				}
			}
		}
	}

	//アニメーターコンポーネント
	private void AnimaterComponent()
	{

		//親オブジェクトのコンポーネントを移行
		Component[] parentComponents = _oldObject.GetComponents<Component>();
		foreach (var component in parentComponents)
		{
			if (_oldObject.GetComponent(component.GetType()).GetType() == typeof(Animator))
			{
				if (_allAnimator)
				{
					UnityEditorInternal.ComponentUtility.CopyComponent(_oldObject.GetComponent(component.GetType()));
					if (_newObject.GetComponent(component.GetType()))
					{
						UnityEditorInternal.ComponentUtility.PasteComponentValues(_newObject.GetComponent(component.GetType()));
					}
					else
					{
						UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_newObject);
					}
				}
				else
				{
					if (!_newObject.GetComponent<Animator>()) _newObject.AddComponent<Animator>();
					if (_cmtAnimator.controller) _newObject.GetComponent<Animator>().runtimeAnimatorController = _oldObject.GetComponent<Animator>().runtimeAnimatorController;
					if (_cmtAnimator.avatar) _newObject.GetComponent<Animator>().avatar = _oldObject.GetComponent<Animator>().avatar;
					if (_cmtAnimator.applyRootMotion) _newObject.GetComponent<Animator>().applyRootMotion = _oldObject.GetComponent<Animator>().applyRootMotion;
					if (_cmtAnimator.updateMode) _newObject.GetComponent<Animator>().updateMode = _oldObject.GetComponent<Animator>().updateMode;
					if (_cmtAnimator.cullingMode) _newObject.GetComponent<Animator>().cullingMode = _oldObject.GetComponent<Animator>().cullingMode;
				}
			}
		}

		//子・孫オブジェクトのコンポーネントを移行
		GameObject[] gameObjects = GetAllChildren.GetAll(_oldObject);
		foreach (var gameObject in gameObjects)
		{
			string objectPath = GetHierarchyPath(gameObject);
			GameObject newObjectCheck = GameObject.Find(_newObject.name + "/" + objectPath)?.gameObject;
			if (newObjectCheck)
			{
				Component[] components = gameObject.GetComponents<Component>();
				foreach (var component in components)
				{
					Component oldComponent = gameObject.GetComponent(component.GetType());
					if (oldComponent.GetType() == typeof(Animator))
					{
						if (_allAnimator)
						{
							UnityEditorInternal.ComponentUtility.CopyComponent(oldComponent);
							if (newObjectCheck.GetComponent(component.GetType()))
							{
								UnityEditorInternal.ComponentUtility.PasteComponentValues(newObjectCheck.GetComponent(component.GetType()));
							}
							else
							{
								UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newObjectCheck);
							}
						}
						else
						{
							if (!newObjectCheck.GetComponent<Animator>()) newObjectCheck.AddComponent<Animator>();
							if (_cmtAnimator.controller) newObjectCheck.GetComponent<Animator>().runtimeAnimatorController = gameObject.GetComponent<Animator>().runtimeAnimatorController;
							if (_cmtAnimator.avatar) newObjectCheck.GetComponent<Animator>().avatar = gameObject.GetComponent<Animator>().avatar;
							if (_cmtAnimator.applyRootMotion) newObjectCheck.GetComponent<Animator>().applyRootMotion = gameObject.GetComponent<Animator>().applyRootMotion;
							if (_cmtAnimator.updateMode) newObjectCheck.GetComponent<Animator>().updateMode = gameObject.GetComponent<Animator>().updateMode;
							if (_cmtAnimator.cullingMode) newObjectCheck.GetComponent<Animator>().cullingMode = gameObject.GetComponent<Animator>().cullingMode;
						}
					}
				}
			}
		}
	}

	//スキンメッシュレンダラーコンポーネント
	private void SkinnedMeshRenderer()
	{
		//親オブジェクトのコンポーネントを移行
		Component[] parentComponents = _oldObject.GetComponents<Component>();
		foreach (var component in parentComponents)
		{
			if (_oldObject.GetComponent(component.GetType()).GetType() == typeof(SkinnedMeshRenderer))
			{
				if (_allSkinnedMeshRenderer)
				{
					if (!_newObject.GetComponent<SkinnedMeshRenderer>()) _newObject.AddComponent<SkinnedMeshRenderer>();
					_newObject.GetComponent<SkinnedMeshRenderer>().localBounds = _oldObject.GetComponent<SkinnedMeshRenderer>().localBounds;
					_newObject.GetComponent<SkinnedMeshRenderer>().quality = _oldObject.GetComponent<SkinnedMeshRenderer>().quality;
					_newObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = _oldObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen;
					_newObject.GetComponent<SkinnedMeshRenderer>().rootBone = _oldObject.GetComponent<SkinnedMeshRenderer>().rootBone;
					_newObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = _oldObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
					_newObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials = _oldObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
					_newObject.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = _oldObject.GetComponent<SkinnedMeshRenderer>().shadowCastingMode;
					_newObject.GetComponent<SkinnedMeshRenderer>().receiveShadows = _oldObject.GetComponent<SkinnedMeshRenderer>().receiveShadows;
					_newObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage = _oldObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage;
					_newObject.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage = _oldObject.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage;
					_newObject.GetComponent<SkinnedMeshRenderer>().probeAnchor = _oldObject.GetComponent<SkinnedMeshRenderer>().probeAnchor;
					_newObject.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors = _oldObject.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors;
					_newObject.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic = _oldObject.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic;
				}
				else
				{
					if (!_newObject.GetComponent<SkinnedMeshRenderer>()) _newObject.AddComponent<SkinnedMeshRenderer>();
					if (_cmtSkinnedMeshRenderer.bounds) _newObject.GetComponent<SkinnedMeshRenderer>().localBounds = _oldObject.GetComponent<SkinnedMeshRenderer>().localBounds;
					if (_cmtSkinnedMeshRenderer.quality) _newObject.GetComponent<SkinnedMeshRenderer>().quality = _oldObject.GetComponent<SkinnedMeshRenderer>().quality;
					if (_cmtSkinnedMeshRenderer.updateWhenOffscreen) _newObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = _oldObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen;
					if (_cmtSkinnedMeshRenderer.rootBone) _newObject.GetComponent<SkinnedMeshRenderer>().rootBone = _oldObject.GetComponent<SkinnedMeshRenderer>().rootBone;
					if (_cmtSkinnedMeshRenderer.mesh) _newObject.GetComponent<SkinnedMeshRenderer>().sharedMesh = _oldObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
					if (_cmtSkinnedMeshRenderer.material) _newObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials = _oldObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
					if (_cmtSkinnedMeshRenderer.castShadows) _newObject.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = _oldObject.GetComponent<SkinnedMeshRenderer>().shadowCastingMode;
					if (_cmtSkinnedMeshRenderer.receiveShadows) _newObject.GetComponent<SkinnedMeshRenderer>().receiveShadows = _oldObject.GetComponent<SkinnedMeshRenderer>().receiveShadows;
					if (_cmtSkinnedMeshRenderer.lightProbes) _newObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage = _oldObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage;
					if (_cmtSkinnedMeshRenderer.reflectionProbes) _newObject.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage = _oldObject.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage;
					if (_cmtSkinnedMeshRenderer.anchorOverride) _newObject.GetComponent<SkinnedMeshRenderer>().probeAnchor = _oldObject.GetComponent<SkinnedMeshRenderer>().probeAnchor;
					if (_cmtSkinnedMeshRenderer.motionVectors) _newObject.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors = _oldObject.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors;
					if (_cmtSkinnedMeshRenderer.dynamicOcdusion) _newObject.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic = _oldObject.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic;
				}
			}
		}

		//子・孫オブジェクトのコンポーネントを移行
		GameObject[] gameObjects = GetAllChildren.GetAll(_oldObject);
		foreach (var gameObject in gameObjects)
		{
			string objectPath = GetHierarchyPath(gameObject);
			GameObject newObjectCheck = GameObject.Find(_newObject.name + "/" + objectPath)?.gameObject;
			if (newObjectCheck)
			{
				Component[] components = gameObject.GetComponents<Component>();
				foreach (var component in components)
				{
					Component oldComponent = gameObject.GetComponent(component.GetType());
					if (oldComponent.GetType() == typeof(SkinnedMeshRenderer))
					{
						if (_allSkinnedMeshRenderer)
						{
							if (!newObjectCheck.GetComponent<SkinnedMeshRenderer>())
							{
								newObjectCheck.AddComponent<SkinnedMeshRenderer>();
							}
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().localBounds = gameObject.GetComponent<SkinnedMeshRenderer>().localBounds;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().quality = gameObject.GetComponent<SkinnedMeshRenderer>().quality;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = gameObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen;
							if (_boneCheck) newObjectCheck.GetComponent<SkinnedMeshRenderer>().rootBone = gameObject.GetComponent<SkinnedMeshRenderer>().rootBone;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().sharedMesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().sharedMaterials = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = gameObject.GetComponent<SkinnedMeshRenderer>().shadowCastingMode;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().receiveShadows = gameObject.GetComponent<SkinnedMeshRenderer>().receiveShadows;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().lightProbeUsage = gameObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage;
							if (gameObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage == UnityEngine.Rendering.LightProbeUsage.UseProxyVolume)
							{
								newObjectCheck.GetComponent<SkinnedMeshRenderer>().lightProbeProxyVolumeOverride = gameObject.GetComponent<SkinnedMeshRenderer>().lightProbeProxyVolumeOverride;
							}
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage = gameObject.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage;
							if (_anchorOverride) newObjectCheck.GetComponent<SkinnedMeshRenderer>().probeAnchor = gameObject.GetComponent<SkinnedMeshRenderer>().probeAnchor;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors = gameObject.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors;
							newObjectCheck.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic = gameObject.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic;
						}
						else
						{
							if (!newObjectCheck.GetComponent<SkinnedMeshRenderer>())
							{
								newObjectCheck.AddComponent<SkinnedMeshRenderer>();
							}
							if (_cmtSkinnedMeshRenderer.bounds)  newObjectCheck.GetComponent<SkinnedMeshRenderer>().localBounds = gameObject.GetComponent<SkinnedMeshRenderer>().localBounds;
							if (_cmtSkinnedMeshRenderer.quality) newObjectCheck.GetComponent<SkinnedMeshRenderer>().quality = gameObject.GetComponent<SkinnedMeshRenderer>().quality;
							if (_cmtSkinnedMeshRenderer.updateWhenOffscreen) newObjectCheck.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen = gameObject.GetComponent<SkinnedMeshRenderer>().updateWhenOffscreen;
							if (_cmtSkinnedMeshRenderer.rootBone) if (_boneCheck) newObjectCheck.GetComponent<SkinnedMeshRenderer>().rootBone = gameObject.GetComponent<SkinnedMeshRenderer>().rootBone;
							if (_cmtSkinnedMeshRenderer.mesh) newObjectCheck.GetComponent<SkinnedMeshRenderer>().sharedMesh = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMesh;
							if (_cmtSkinnedMeshRenderer.material) newObjectCheck.GetComponent<SkinnedMeshRenderer>().sharedMaterials = gameObject.GetComponent<SkinnedMeshRenderer>().sharedMaterials;
							if (_cmtSkinnedMeshRenderer.castShadows) newObjectCheck.GetComponent<SkinnedMeshRenderer>().shadowCastingMode = gameObject.GetComponent<SkinnedMeshRenderer>().shadowCastingMode;
							if (_cmtSkinnedMeshRenderer.receiveShadows) newObjectCheck.GetComponent<SkinnedMeshRenderer>().receiveShadows = gameObject.GetComponent<SkinnedMeshRenderer>().receiveShadows;
							if (_cmtSkinnedMeshRenderer.lightProbes) newObjectCheck.GetComponent<SkinnedMeshRenderer>().lightProbeUsage = gameObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage;
							if (gameObject.GetComponent<SkinnedMeshRenderer>().lightProbeUsage == UnityEngine.Rendering.LightProbeUsage.UseProxyVolume)
							{
								newObjectCheck.GetComponent<SkinnedMeshRenderer>().lightProbeProxyVolumeOverride = gameObject.GetComponent<SkinnedMeshRenderer>().lightProbeProxyVolumeOverride;
							}
							if (_cmtSkinnedMeshRenderer.reflectionProbes) newObjectCheck.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage = gameObject.GetComponent<SkinnedMeshRenderer>().reflectionProbeUsage;
							if (_cmtSkinnedMeshRenderer.anchorOverride) if (_anchorOverride) newObjectCheck.GetComponent<SkinnedMeshRenderer>().probeAnchor = gameObject.GetComponent<SkinnedMeshRenderer>().probeAnchor;
							if (_cmtSkinnedMeshRenderer.motionVectors) newObjectCheck.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors = gameObject.GetComponent<SkinnedMeshRenderer>().skinnedMotionVectors;
							if (_cmtSkinnedMeshRenderer.dynamicOcdusion) newObjectCheck.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic = gameObject.GetComponent<SkinnedMeshRenderer>().allowOcclusionWhenDynamic;
						}
					}
				}
			}
		}
	}

	//メッシュレンダラーコンポーネント
	private void MeshRendererComponent()
	{
		//親オブジェクトのコンポーネントを移行
		Component[] parentComponents = _oldObject.GetComponents<Component>();
		foreach (var component in parentComponents)
		{
			if (_oldObject.GetComponent(component.GetType()).GetType() == typeof(MeshRenderer))
			{
				if (_allMeshRenderer)
				{
					UnityEditorInternal.ComponentUtility.CopyComponent(_oldObject.GetComponent(component.GetType()));
					if (_newObject.GetComponent(component.GetType()))
					{
						UnityEditorInternal.ComponentUtility.PasteComponentValues(_newObject.GetComponent(component.GetType()));
					}
					else
					{
						UnityEditorInternal.ComponentUtility.PasteComponentAsNew(_newObject);
					}
				}
				else
				{
					if (!_newObject.GetComponent<MeshRenderer>()) _newObject.AddComponent<MeshRenderer>();
					if (_cmtMeshRenderer.material) _newObject.GetComponent<MeshRenderer>().sharedMaterials = _oldObject.GetComponent<MeshRenderer>().sharedMaterials;
					if (_cmtMeshRenderer.castShadows) _newObject.GetComponent<MeshRenderer>().shadowCastingMode = _oldObject.GetComponent<MeshRenderer>().shadowCastingMode;
					if (_cmtMeshRenderer.receiveShadows) _newObject.GetComponent<MeshRenderer>().receiveShadows = _oldObject.GetComponent<MeshRenderer>().receiveShadows;
					if (_cmtMeshRenderer.contributeGlobalIllumination) _newObject.GetComponent<MeshRenderer>().lightmapIndex = _oldObject.GetComponent<MeshRenderer>().lightmapIndex;
					if (_cmtMeshRenderer.receiveGlobalIllumination) _newObject.GetComponent<MeshRenderer>().receiveGI = _oldObject.GetComponent<MeshRenderer>().receiveGI;
					if (_cmtMeshRenderer.lightProbes) _newObject.GetComponent<MeshRenderer>().lightProbeUsage = _oldObject.GetComponent<MeshRenderer>().lightProbeUsage;
					if (_cmtMeshRenderer.lightProbes && _newObject.GetComponent<MeshRenderer>().lightProbeUsage == UnityEngine.Rendering.LightProbeUsage.UseProxyVolume)
					{
						_newObject.GetComponent<MeshRenderer>().lightProbeProxyVolumeOverride = _oldObject.GetComponent<MeshRenderer>().lightProbeProxyVolumeOverride;
					}
					if (_cmtMeshRenderer.reflectionProbes) _newObject.GetComponent<MeshRenderer>().reflectionProbeUsage = _oldObject.GetComponent<MeshRenderer>().reflectionProbeUsage;
					if (_cmtMeshRenderer.anchorOverride) _newObject.GetComponent<MeshRenderer>().probeAnchor = _oldObject.GetComponent<MeshRenderer>().probeAnchor;
					if (_cmtMeshRenderer.motionVectors) _newObject.GetComponent<MeshRenderer>().motionVectorGenerationMode = _oldObject.GetComponent<MeshRenderer>().motionVectorGenerationMode;
					if (_cmtMeshRenderer.dynamicOcdusion) _newObject.GetComponent<MeshRenderer>().allowOcclusionWhenDynamic = _oldObject.GetComponent<MeshRenderer>().allowOcclusionWhenDynamic;
				}
			}
		}

		//子・孫オブジェクトのコンポーネントを移行
		GameObject[] gameObjects = GetAllChildren.GetAll(_oldObject);
		foreach (var gameObject in gameObjects)
		{
			string objectPath = GetHierarchyPath(gameObject);
			GameObject newObjectCheck = GameObject.Find(_newObject.name + "/" + objectPath)?.gameObject;
			if (newObjectCheck)
			{
				Component[] components = gameObject.GetComponents<Component>();
				foreach (var component in components)
				{
					Component oldComponent = gameObject.GetComponent(component.GetType());
					if (oldComponent.GetType() == typeof(MeshRenderer))
					{
						if (_allMeshRenderer)
						{
							UnityEditorInternal.ComponentUtility.CopyComponent(oldComponent);
							if (newObjectCheck.GetComponent(component.GetType()))
							{
								UnityEditorInternal.ComponentUtility.PasteComponentValues(newObjectCheck.GetComponent(component.GetType()));
							}
							else
							{
								UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newObjectCheck);
							}
						}
						else
						{
							if (!newObjectCheck.GetComponent<MeshRenderer>()) newObjectCheck.AddComponent<MeshRenderer>();
							if (_cmtMeshRenderer.material) newObjectCheck.GetComponent<MeshRenderer>().sharedMaterials = gameObject.GetComponent<MeshRenderer>().sharedMaterials;
							if (_cmtMeshRenderer.castShadows) newObjectCheck.GetComponent<MeshRenderer>().shadowCastingMode = gameObject.GetComponent<MeshRenderer>().shadowCastingMode;
							if (_cmtMeshRenderer.receiveShadows) newObjectCheck.GetComponent<MeshRenderer>().receiveShadows = gameObject.GetComponent<MeshRenderer>().receiveShadows;
							if (_cmtMeshRenderer.contributeGlobalIllumination) newObjectCheck.GetComponent<MeshRenderer>().lightmapIndex = gameObject.GetComponent<MeshRenderer>().lightmapIndex;
							if (_cmtMeshRenderer.receiveGlobalIllumination) newObjectCheck.GetComponent<MeshRenderer>().receiveGI = gameObject.GetComponent<MeshRenderer>().receiveGI;
							if (_cmtMeshRenderer.lightProbes) newObjectCheck.GetComponent<MeshRenderer>().lightProbeUsage = gameObject.GetComponent<MeshRenderer>().lightProbeUsage;
							if(_cmtMeshRenderer.lightProbes && newObjectCheck.GetComponent<MeshRenderer>().lightProbeUsage == UnityEngine.Rendering.LightProbeUsage.UseProxyVolume)
							{
								newObjectCheck.GetComponent<MeshRenderer>().lightProbeProxyVolumeOverride = gameObject.GetComponent<MeshRenderer>().lightProbeProxyVolumeOverride;
							}
							if (_cmtMeshRenderer.reflectionProbes) newObjectCheck.GetComponent<MeshRenderer>().reflectionProbeUsage = gameObject.GetComponent<MeshRenderer>().reflectionProbeUsage;
							if (_cmtMeshRenderer.anchorOverride) newObjectCheck.GetComponent<MeshRenderer>().probeAnchor = gameObject.GetComponent<MeshRenderer>().probeAnchor;
							if (_cmtMeshRenderer.motionVectors) newObjectCheck.GetComponent<MeshRenderer>().motionVectorGenerationMode = gameObject.GetComponent<MeshRenderer>().motionVectorGenerationMode;
							if (_cmtMeshRenderer.dynamicOcdusion) newObjectCheck.GetComponent<MeshRenderer>().allowOcclusionWhenDynamic = gameObject.GetComponent<MeshRenderer>().allowOcclusionWhenDynamic;
						}
					}
				}
			}
		}
	}
}

//孫オブジェクトまで取得する
public static class GetAllChildren
{
	//リストから配列に変更した
	public static GameObject[] GetAll(this GameObject obj)
	{
		List<GameObject> allChildren = new List<GameObject>();
		GetChildren(obj, ref allChildren);
		return allChildren.ToArray();
	}

	public static void GetChildren(GameObject obj, ref List<GameObject> allChildren)
	{
		Transform children = obj.GetComponentInChildren<Transform>();
		if (children.childCount == 0)
		{
			return;
		}
		foreach (Transform ob in children)
		{
			allChildren.Add(ob.gameObject);
			GetChildren(ob.gameObject, ref allChildren);
		}
	}
}
