using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkinManager : MonoBehaviour, IEnumerable<SkinManager.ImageInfo>, ISerializationCallbackReceiver
{
    [SerializeField]
    private EntityBase _entity;
    public EntityBase Entity => _entity != null ? _entity : _entity = this.SearchComponent<EntityBase>();

    [SerializeField]
    private ImageInfo[] _images;

    public ImageInfo this[int index] => _images[index];

    private void Start()
    {
        ReplaceColors();
    }

    public void ReplaceColors()
    {
        ReplaceColors(Entity.TeamColor);
    }

    public void ReplaceColors(Color newColor)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif
        for (int i = _images.Length - 1; i > -1; --i)
        {
            _images[i].ReplaceColorOfSprites(newColor);
        }
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(this);
        }
#endif
    }

    public void ReplaceWithOriginalColor()
    {
        for (int i = _images.Length - 1; i > -1; --i)
        {
            _images[i].ReplaceWithOriginal();
        }
    }

    public void AddRenderer(ImageInfo imageInfo)
    {
        System.Array.Resize(ref _images, _images.Length + 1);
        _images[_images.Length - 1] = imageInfo;
    }
    public void RemoveRenderer(SpriteRenderer renderer)
    {
        MyLib.ArrayRemoveOne(ref _images, x => x.Renderer == renderer);
    }

    public IEnumerator<ImageInfo> GetEnumerator()
    {
        return ((IEnumerable<ImageInfo>)_images).GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void OnBeforeSerialize()
    {
        for (int i = 0; i < _images.Length; ++i)
        {
            if (!_images[i].HasManySprites)
            {
                if (_images[i].Renderer == null) continue;
                var multipleSpritesObject = _images[i].Renderer.GetComponent<IMultipleSprites>();
                if (multipleSpritesObject != null)
                {
                    _images[i] = new ImageInfoMultipleSprites(_images[i], multipleSpritesObject);
                }
                else
                {
                    _images[i] = new ImageInfo(_images[i].Renderer, _images[i].OriginalImage, _images[i].CurrentColor);
                }
            }
        }
    }

    public void OnAfterDeserialize()
    {
        for (int i = 0; i < _images.Length; ++i)
        {
            if (_images[i].HasManySprites)
            {
                _images[i] = new ImageInfoMultipleSprites(_images[i]);
            }
        }
    }

    public interface IMultipleSprites : IEnumerable<Sprite>
    {
        void SetSprites(Sprite[] sprites);
    }

    [Serializable]
    public class ImageInfo : ISerializationCallbackReceiver
    {
        [SerializeField, ReadOnlyOnInspector]
        protected bool _hasManySprites = false;
        public bool HasManySprites => _hasManySprites;

        [SerializeField, HideInInspector]
        protected SpriteRenderer _renderer;
        public SpriteRenderer Renderer => _renderer;

        [SerializeField, ReadOnlyOnInspector]
        private Sprite _originalImage = null;
        public Sprite OriginalImage => _originalImage;


        [SerializeField, HideInInspector]
        protected Sprite[] _originalImages;


        [SerializeField, ReadOnlyOnInspector]
        protected Color _currentColor = MyColorLib.BASE_COLOR_TO_REPLACE;

        public Color CurrentColor
        {
            get => _currentColor.a != 1f ? _currentColor = new Color(_currentColor.r, _currentColor.g, _currentColor.b, 1f) : _currentColor;
            protected set => _currentColor = value.a != 1f ? new Color(value.r, value.g, value.b, 1f) : value;
        }

        public ImageInfo(SpriteRenderer renderer)
        {
            _renderer = renderer;
            _originalImage = renderer.sprite;
        }
        public ImageInfo(SpriteRenderer renderer, Sprite originalImage, Color currentColor)
        {
            _renderer = renderer;
            _originalImage = originalImage;
            CurrentColor = currentColor;
        }
        public ImageInfo(ImageInfo original) : this(original.Renderer, null, original.CurrentColor)
        {
            if (original.HasManySprites)
            {
                _originalImages = original._originalImages;
            }
        }
        public virtual void ReplaceWithOriginal()
        {
            if (Renderer == null) return;
            CurrentColor = MyColorLib.BASE_COLOR_TO_REPLACE;
            if (_originalImage != null)
            {
                _renderer.sprite = _originalImage;
            }
            else
            {
                _originalImage = _renderer.sprite;
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(_renderer);
            }
#endif
        }
        public virtual void ReplaceColorOfSprites(Color newColor)
        {
            if (Renderer == null) return;
            _renderer.sprite = MyColorLib.GetSpriteColored(CurrentColor = newColor, _originalImage != null ? _originalImage : _originalImage = _renderer.sprite);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(_renderer);
            }
#endif
        }

        public void OnBeforeSerialize()
        {
            if (_originalImage == null && _renderer != null) _originalImage = _renderer.sprite;
            if (_currentColor == new Color(0f, 0f, 0f, 0f)) _currentColor = MyColorLib.BASE_COLOR_TO_REPLACE;
            else if (_currentColor.a != 1f) _currentColor = new Color(_currentColor.r, _currentColor.g, _currentColor.b, 1f);
        }

        public void OnAfterDeserialize()
        {
        }

#if UNITY_EDITOR

        [UnityEditor.CustomPropertyDrawer(typeof(ImageInfo))]
        private class SkinManagerPropertyDrawer : UnityEditor.PropertyDrawer
        {
            private const float _FIELD_HEIGHT = 20f, _NEXT_FIELD_HEIGHT = _FIELD_HEIGHT + 3f, _COLOR_FIELD_WIDTH = 50f, _NEXT_COLOR_FIELD_WIDTH = _COLOR_FIELD_WIDTH / 2f + 5f;
            private const int _NUMBER_OF_FIELDS_DISPLAYED = 5;
            public override float GetPropertyHeight(UnityEditor.SerializedProperty property, GUIContent label)
            {
                int extras = property.FindPropertyRelative(nameof(_originalImages)).arraySize;
                return base.GetPropertyHeight(property, label) * (2 + _NUMBER_OF_FIELDS_DISPLAYED + (extras != 0 ? 1.3f * extras : 0));
            }



            public override void OnGUI(Rect position, UnityEditor.SerializedProperty property, GUIContent label)
            {
                UnityEditor.EditorGUI.BeginProperty(position, label, property);

                var fieldLabel = new GUIStyle();
                fieldLabel.alignment = TextAnchor.UpperLeft;

                UnityEditor.EditorGUI.LabelField(position, label, fieldLabel);

                var currentColorProperty = property.FindPropertyRelative(nameof(_currentColor));
                Color currentColor = currentColorProperty.colorValue;

                var rendererRect = new Rect(position.x, position.y + _NEXT_FIELD_HEIGHT, position.width, _FIELD_HEIGHT);
                var spriteRect = new Rect(position.x, position.y + _NEXT_FIELD_HEIGHT * 2, position.width, _FIELD_HEIGHT);

                UnityEditor.EditorGUI.indentLevel++;


                UnityEditor.EditorGUI.PropertyField(rendererRect, property.FindPropertyRelative(nameof(_renderer)));

                UnityEditor.EditorGUI.BeginDisabledGroup(true);
                int extraHeight = 0;
                if (!property.FindPropertyRelative(nameof(_hasManySprites)).boolValue)
                {
                    UnityEditor.EditorGUI.PropertyField(spriteRect, property.FindPropertyRelative(nameof(_originalImage)));
                }
                else
                {
                    var arrayProperty = property.FindPropertyRelative(nameof(_originalImages));
                    int length = arrayProperty.arraySize;
                    extraHeight = length;
                    UnityEditor.EditorGUI.IntField(spriteRect, "Number of original images", length);
                    UnityEditor.EditorGUI.indentLevel++;
                    for (int i = 0; i < length; ++i)
                    {
                        spriteRect.y += _NEXT_FIELD_HEIGHT;
                        UnityEditor.EditorGUI.PropertyField(spriteRect, arrayProperty.GetArrayElementAtIndex(i));
                    }

                    UnityEditor.EditorGUI.indentLevel--;
                }

                var colorRect = new Rect(position.x, position.y + _NEXT_FIELD_HEIGHT * (3+ extraHeight), position.width, _FIELD_HEIGHT);
                var x = Screen.width * 0.35f;

                float colorValuesRectHeight = position.y + _NEXT_FIELD_HEIGHT * (4 + extraHeight);

                var RRect = new Rect(x, colorValuesRectHeight, position.width, _FIELD_HEIGHT);
                var RFieldRect = new Rect(x + _NEXT_COLOR_FIELD_WIDTH * 1, colorValuesRectHeight, _COLOR_FIELD_WIDTH, _FIELD_HEIGHT);
                var GRect = new Rect(x + _NEXT_COLOR_FIELD_WIDTH * 3, colorValuesRectHeight, position.width, _FIELD_HEIGHT);
                var GFieldRect = new Rect(x + _NEXT_COLOR_FIELD_WIDTH * 4, colorValuesRectHeight, _COLOR_FIELD_WIDTH, _FIELD_HEIGHT);
                var BRect = new Rect(x + _NEXT_COLOR_FIELD_WIDTH * 6, colorValuesRectHeight, position.width, _FIELD_HEIGHT);
                var BFieldRect = new Rect(x + _NEXT_COLOR_FIELD_WIDTH * 7, colorValuesRectHeight, _COLOR_FIELD_WIDTH, _FIELD_HEIGHT);
                var ARect = new Rect(x + _NEXT_COLOR_FIELD_WIDTH * 9, colorValuesRectHeight, position.width, _FIELD_HEIGHT);
                var AFieldRect = new Rect(x + _NEXT_COLOR_FIELD_WIDTH * 10, colorValuesRectHeight, _COLOR_FIELD_WIDTH, _FIELD_HEIGHT);

                UnityEditor.EditorGUI.PropertyField(colorRect, currentColorProperty);

                UnityEditor.EditorGUI.indentLevel--;


                UnityEditor.EditorGUI.LabelField(RRect, "R", UnityEditor.EditorStyles.label);
                UnityEditor.EditorGUI.TextField(RFieldRect, (currentColor.r * 255f).ToString("0"));
                UnityEditor.EditorGUI.LabelField(GRect, "G", UnityEditor.EditorStyles.label);
                UnityEditor.EditorGUI.TextField(GFieldRect, (currentColor.g * 255f).ToString("0"));
                UnityEditor.EditorGUI.LabelField(BRect, "B", UnityEditor.EditorStyles.label);
                UnityEditor.EditorGUI.TextField(BFieldRect, (currentColor.b * 255f).ToString("0"));
                UnityEditor.EditorGUI.LabelField(ARect, "A", UnityEditor.EditorStyles.label);
                UnityEditor.EditorGUI.TextField(AFieldRect, (currentColor.a * 255f).ToString("0"));

                UnityEditor.EditorGUI.EndDisabledGroup();

                UnityEditor.EditorGUI.indentLevel++;
                UnityEditor.EditorGUI.PropertyField(new Rect(position.x, position.y + _NEXT_FIELD_HEIGHT * (5 + extraHeight), position.width, _FIELD_HEIGHT), property.FindPropertyRelative(nameof(_hasManySprites)));
                UnityEditor.EditorGUI.indentLevel--;

                UnityEditor.EditorGUI.EndProperty();

            }

        }
#endif
    }

    public class ImageInfoMultipleSprites : ImageInfo
    {
        private IMultipleSprites _multipleSpritesObjectField;

        private IMultipleSprites _multipleSpritesObject
        {
            get
            {
                if (Renderer == null) return null;
                return _multipleSpritesObjectField != null ? _multipleSpritesObjectField : _multipleSpritesObjectField = Renderer.GetComponent<IMultipleSprites>();
            }
            set => _multipleSpritesObjectField = value;
        }

        public ImageInfoMultipleSprites(ImageInfo original, IEnumerable<Sprite> originalSprites, IMultipleSprites multipleSpritesObject = null) : base(original)
        {
            _multipleSpritesObject = multipleSpritesObject;
            if ((_originalImages == null || _originalImages.Length != 0) && originalSprites != null)
            {
                _originalImages = new List<Sprite>(originalSprites).ToArray();
            }
            _hasManySprites = true;
        }
        public ImageInfoMultipleSprites(ImageInfo original, IMultipleSprites multipleSpritesObject = null) : this(original, multipleSpritesObject, multipleSpritesObject)
        {
        }

        public override void ReplaceWithOriginal()
        {
            if (Renderer == null) return;
            CurrentColor = MyColorLib.BASE_COLOR_TO_REPLACE;
            if (_originalImages == null || _originalImages.Length == 0)
            {
                _originalImages = new List<Sprite>(_multipleSpritesObject).ToArray();
            }
            else
            {
                Sprite[] newSprites = new Sprite[_originalImages.Length];
                int currentSpriteIndex = 0;
                foreach (Sprite sprite in _multipleSpritesObject)
                {
                    if (Renderer.sprite == sprite) break;
                    currentSpriteIndex++;
                }
                if (currentSpriteIndex >= _originalImages.Length)
                {
#if UNITY_EDITOR
                    Debug.Log("hmm currentSpriteIndex >= _originalSprites.Length, either sprites were added and not recorded here, or current sprite not in sprites group "
                        + currentSpriteIndex + " " + _originalImages.Length, Renderer);
#endif
                    for (int i = _originalImages.Length - 1; i >= 0; --i)
                    {
                        newSprites[i] = _originalImages[i];
                    }
                }
                else
                {
                    Renderer.sprite = newSprites[currentSpriteIndex] = _originalImages[currentSpriteIndex];
                    int i = _originalImages.Length - 1;
                    for (; i > currentSpriteIndex; --i)
                    {
                        newSprites[i] = _originalImages[i];
                    }
                    for (--i; i >= 0; --i)
                    {
                        newSprites[i] = _originalImages[i];
                    }
                }
                _multipleSpritesObject.SetSprites(newSprites);
            }
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(_renderer);
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(_multipleSpritesObject as MonoBehaviour);
            }
#endif
        }
        public override void ReplaceColorOfSprites(Color newColor)
        {
            if (Renderer == null) return;
            Sprite[] newSprites;
            if (_originalImages != null && _originalImages.Length != 0)
            {
                newSprites = new Sprite[_originalImages.Length];
                int currentSpriteIndex = 0;
                foreach (Sprite sprite in _multipleSpritesObject)
                {
                    if (Renderer.sprite == sprite) break;
                    currentSpriteIndex++;
                }
                if(currentSpriteIndex >= _originalImages.Length)
                {
                    Debug.Log("hmm currentSpriteIndex >= _originalSprites.Length, either sprites were added and not recorded here, or current sprite not in sprites group "
                        + currentSpriteIndex + " " + _originalImages.Length, Renderer);
                    _renderer.sprite = MyColorLib.GetSpriteColored(CurrentColor = newColor, _renderer.sprite);
                    for (int i = _originalImages.Length - 1; i >= 0; --i)
                    {
                        newSprites[i] = MyColorLib.GetSpriteColored(CurrentColor, _originalImages[i]);
                    }
                }
                else
                {
                    Renderer.sprite = newSprites[currentSpriteIndex] = MyColorLib.GetSpriteColored(CurrentColor = newColor, _originalImages[currentSpriteIndex]);
                    int i = _originalImages.Length - 1;
                    for (; i > currentSpriteIndex; --i)
                    {
                        newSprites[i] = MyColorLib.GetSpriteColored(CurrentColor, _originalImages[i]);
                    }
                    for (--i; i >= 0; --i)
                    {
                        newSprites[i] = MyColorLib.GetSpriteColored(CurrentColor, _originalImages[i]);
                    }
                }
            }
            else
            {
                newSprites = new Sprite[(_originalImages = new List<Sprite>(_multipleSpritesObject).ToArray()).Length];
                _renderer.sprite = MyColorLib.GetSpriteColored(CurrentColor = newColor, _renderer.sprite);
                for (int i = _originalImages.Length - 1; i >= 0; --i)
                {
                    newSprites[i] = MyColorLib.GetSpriteColored(CurrentColor, _originalImages[i]);
                }
            }
            _multipleSpritesObject.SetSprites(newSprites);
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(_renderer);
                UnityEditor.PrefabUtility.RecordPrefabInstancePropertyModifications(_multipleSpritesObject as MonoBehaviour);
            }
#endif
        }

    }

}
