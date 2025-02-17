﻿using System;
using System.Runtime.Serialization;
using Umbraco.Cms.Core.Models.Entities;

namespace Umbraco.Cms.Core.Models
{
    /// <summary>
    /// Represents a translation for a <see cref="DictionaryItem"/>
    /// </summary>
    [Serializable]
    [DataContract(IsReference = true)]
    public class DictionaryTranslation : EntityBase, IDictionaryTranslation
    {
        public Func<int, ILanguage> GetLanguage { get; set; }

        private ILanguage _language;
        private string _value;
        //note: this will be memberwise cloned
        private int _languageId;

        public DictionaryTranslation(ILanguage language, string value)
        {
            if (language == null) throw new ArgumentNullException("language");
            _language = language;
            _languageId = _language.Id;
            _value = value;
        }

        public DictionaryTranslation(ILanguage language, string value, Guid uniqueId)
        {
            if (language == null) throw new ArgumentNullException("language");
            _language = language;
            _languageId = _language.Id;
            _value = value;
            Key = uniqueId;
        }

        public DictionaryTranslation(int languageId, string value)
        {
            _languageId = languageId;
            _value = value;
        }

        public DictionaryTranslation(int languageId, string value, Guid uniqueId)
        {
            _languageId = languageId;
            _value = value;
            Key = uniqueId;
        }

        /// <summary>
        /// Gets or sets the <see cref="Language"/> for the translation
        /// </summary>
        /// <remarks>
        /// Marked as DoNotClone - TODO: this member shouldn't really exist here in the first place, the DictionaryItem
        /// class will have a deep hierarchy of objects which all get deep cloned which we don't want. This should have simply
        /// just referenced a language ID not the actual language object. In v8 we need to fix this.
        /// We're going to have to do the same hacky stuff we had to do with the Template/File contents so that this is returned
        /// on a callback.
        /// </remarks>
        [DataMember]
        [DoNotClone]
        public ILanguage Language
        {
            get
            {
                if (_language != null)
                    return _language;

                // else, must lazy-load
                if (GetLanguage != null && _languageId > 0)
                    _language = GetLanguage(_languageId);
                return _language;
            }
            set
            {
                SetPropertyValueAndDetectChanges(value, ref _language, nameof(Language));
                _languageId = _language == null ? -1 : _language.Id;
            }
        }

        public int LanguageId
        {
            get { return _languageId; }
        }

        /// <summary>
        /// Gets or sets the translated text
        /// </summary>
        [DataMember]
        public string Value
        {
            get { return _value; }
            set { SetPropertyValueAndDetectChanges(value, ref _value, nameof(Value)); }
        }

        protected override void PerformDeepClone(object clone)
        {
            base.PerformDeepClone(clone);

            var clonedEntity = (DictionaryTranslation)clone;

            // clear fields that were memberwise-cloned and that we don't want to clone
            clonedEntity._language = null;
        }
    }
}
