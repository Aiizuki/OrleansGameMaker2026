using System;
using System.Collections;
using Core.Enums;
using Core.Scripts;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Assets.Components.UI
{
	/// <summary>
	/// Handles scene transitions with fade effects, including loading new scenes, reloading the current scene, and transitioning to game over or win screens
	/// </summary>
	public class SceneTransitionManager : MonoBehaviour
	{
		[SerializeField] private CanvasGroup fadeCanvasGroup;
		[SerializeField] private float fadeDuration = 0.5f;
		[SerializeField] private float blackScreenDuration = 0.3f;

		[SerializeField] private SceneAsset _homeScene;
		[SerializeField] private SceneAsset _gameScene;
		[SerializeField] private SceneAsset _gameOverScene;

		private void Start()
		{
			UnityEventManager.AddListener(nameof(EnumUnityEventName.GameStart), OnNewGame);
		}

		private void OnDestroy()
		{
			UnityEventManager.RemoveListener(nameof(EnumUnityEventName.GameStart), OnNewGame);
		}

		[Obsolete("Not used in this project ATM")]
		public void ReloadCurrentScene()
		{
			StartCoroutine(ReloadSceneRoutine());
		}

		private IEnumerator TransitionToScene(SceneAsset scene)
		{
			yield return StartCoroutine(FadeToBlack());
			yield return new WaitForSeconds(blackScreenDuration);

			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(scene.name);
			while (!asyncLoad.isDone)
				yield return null;

			yield return StartCoroutine(FadeFromBlack());
		}

		private IEnumerator NewGameRoutine()
			=> TransitionToScene(_gameScene);

		private IEnumerator ReturnToHomeRoutine()
			=> TransitionToScene(_homeScene);

		[Obsolete("Not used in this project ATM")]
		private IEnumerator GameWinRoutine()
			=> TransitionToScene(_gameOverScene);

		private IEnumerator LoadGameOverSceneRoutine()
			=> TransitionToScene(_gameOverScene);


		/// <summary>
		/// Reloads the current scene with a fade to black and fade from black effect, ensuring that all sounds are cleaned up before reloading the scene*
		/// Used when the player go to the next floor
		/// </summary>
		[Obsolete("Not used in this project ATM")]
		private IEnumerator ReloadSceneRoutine()
		{
			int sceneIndex = SceneManager.GetActiveScene().buildIndex;

			yield return StartCoroutine(FadeToBlack());

			yield return new WaitForSeconds(blackScreenDuration);

			//AudioManager.Instance.CleanSounds();
			AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
			while (!asyncLoad.isDone)
				yield return null;

			yield return StartCoroutine(FadeFromBlack());
		}

		/// <summary>
		/// Fades the screen to black over the specified duration
		/// </summary>
		/// <returns></returns>
		private IEnumerator FadeToBlack()
		{
			float elapsedTime = 0f;

			while (elapsedTime < fadeDuration)
			{
				elapsedTime += Time.unscaledDeltaTime;
				fadeCanvasGroup.alpha = Mathf.Clamp01(elapsedTime / fadeDuration);
				yield return null;
			}

			fadeCanvasGroup.alpha = 1f;
		}

		/// <summary>
		/// Fades the screen from black to transparent over the specified duration
		/// </summary>
		/// <returns></returns>
		private IEnumerator FadeFromBlack()
		{
			float elapsedTime = 0f;

			while (elapsedTime < fadeDuration)
			{
				elapsedTime += Time.unscaledDeltaTime;
				fadeCanvasGroup.alpha = 1f - Mathf.Clamp01(elapsedTime / fadeDuration);
				yield return null;
			}

			fadeCanvasGroup.alpha = 0f;
		}

		#region Event Handlers

		private void OnReturnToHome()
			=> StartCoroutine(ReturnToHomeRoutine());

		private void OnNewGame()
			=> StartCoroutine(NewGameRoutine());

		private void OnGameOver()
			=> StartCoroutine(LoadGameOverSceneRoutine());

		#endregion Event Handlers
	}
}