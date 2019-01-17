using System.Collections;
using System.Collections.Generic;
using BallGame.View;
using UnityEngine;
using UnityEngine.UI;

namespace BallGame.HUD
{
	public class UnitStatusBar : MonoBehaviour
	{
		
		[SerializeField] Image _barPrefab;
		[SerializeField] RectTransform _container;
		
		readonly List<float> _radiusValuesBuffer = new List<float>();
		readonly List<Color> _colorValuesBuffer = new List<Color>();
		readonly List<Image> _bars = new List<Image>();

		void Awake()
		{
			_barPrefab.gameObject.SetActive(false);
		}

		void Update()
		{
			var simulationView = SimulationView.Instance;
			if(simulationView == null || simulationView.Simulation == null)
				return;
			_radiusValuesBuffer.Clear();
			_colorValuesBuffer.Clear();
			var totalRadius = simulationView.Simulation.GetTotalRadius();
			if(totalRadius == 0)
			{
				SetBars(_radiusValuesBuffer, _colorValuesBuffer);
				return;
			}
			for (int i = 0; i < simulationView.Colors.Length; i++)
			{
				var rad = simulationView.Simulation.GetTotalRadius(i);
				var ratio = rad / totalRadius;
				_radiusValuesBuffer.Add(ratio);
				_colorValuesBuffer.Add(simulationView.Colors[i].color);
			}
			SetBars(_radiusValuesBuffer, _colorValuesBuffer);
		}

		public void SetBars(List<float> values, List<Color> colors)
		{
			while (_bars.Count < values.Count)
			{
				var bar = Instantiate(_barPrefab);
				bar.rectTransform.SetParent(_container);
				bar.rectTransform.localScale = Vector3.one;
				bar.gameObject.SetActive(true);
				_bars.Add(bar);
			}
			
			for (int i = 0; i < _bars.Count; i++)
			{
				if (i < values.Count)
				{
					var bar = _bars[i];
					var size = bar.rectTransform.sizeDelta;
					size.x = values[i] * _container.rect.width;
					bar.rectTransform.sizeDelta = size;
					var pos = bar.rectTransform.localPosition;
					pos.y = 0;
					var x = 0f;
					for (int j = 0; j < i; j++)
					{
						x += values[j] * _container.rect.width;
					}
					pos.x = i > 0 ? x - (_container.rect.width / 2) : -_container.rect.width / 2;
					bar.rectTransform.localPosition = pos;
					bar.color = colors[i];
				}
				else
				{
					_bars[i].color = Color.clear;
				}
			}
		}
	}

}
