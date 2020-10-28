using EmotionPlatzi.Web.Models;
using Microsoft.ProjectOxford.Common.Contract;
using Microsoft.ProjectOxford.Emotion;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;

namespace EmotionPlatzi.Web.Util
{
    public class EmotionHelper
    {
        public EmotionServiceClient emoclient;

        public EmotionHelper(string key)
        {
            emoclient = new EmotionServiceClient(key);
        }
        public async Task<EmoPicture> DetectandExtracFacesAsync(Stream imagestream) 
        {
            Emotion[] emotions = await emoclient.RecognizeAsync(imagestream);
            var emoPicture = new EmoPicture();

            emoPicture.Faces = ExtractFaces(emotions, emoPicture);

            return emoPicture;
        }

        private ObservableCollection<EmoFace> ExtractFaces(Emotion[] emotions, EmoPicture emoPicture)
        {
            var listFaces = new ObservableCollection<EmoFace>();

            foreach (var emotion in emotions)
            {
                var emoFace = new EmoFace()
                {
                    X = emotion.FaceRectangle.Left,
                    Y = emotion.FaceRectangle.Top,
                    Width = emotion.FaceRectangle.Width,
                    Height = emotion.FaceRectangle.Height,
                    Picture = emoPicture
                };
                
                emoFace.Emotions = ProcessEmotion(emotion.Scores, emoFace);
                listFaces.Add(emoFace);

            }
            return listFaces;
        }

        private ObservableCollection<EmoEmotion> ProcessEmotion(EmotionScores scores, EmoFace emoFace)
        {
            var emotionList = new ObservableCollection<EmoEmotion>();
            var properties = scores.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var filterproperties = properties.Where(p=> p.PropertyType == typeof(float));

            var emotype = EmoEmotionEnum.Undeterminated;

            foreach (var prop in filterproperties)
            {
                if (!Enum.TryParse<EmoEmotionEnum>(prop.Name, out emotype))
                    emotype = EmoEmotionEnum.Undeterminated;

                var emoEmotion = new EmoEmotion();
                emoEmotion.Score = (float)prop.GetValue(scores);
                emoEmotion.EmotionType = emotype;
                emoEmotion.Face = emoFace;

                emotionList.Add(emoEmotion);
            }
            return emotionList;

        }
    }
}