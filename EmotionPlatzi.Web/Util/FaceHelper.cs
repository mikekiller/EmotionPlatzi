using EmotionPlatzi.Web.Controllers;
using EmotionPlatzi.Web.Models;
using Microsoft.Azure.CognitiveServices.Vision.Face;
using Microsoft.Azure.CognitiveServices.Vision.Face.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;


namespace EmotionPlatzi.Web.Util
{
    public class FaceHelper
    {
        private readonly IFaceClient faceClient;

        public FaceHelper(string key , string endPoint)
        {
            faceClient = new FaceClient(
                    new ApiKeyServiceClientCredentials(key),
                    new DelegatingHandler[] { }
                );
            faceClient.Endpoint = endPoint;
        }
        public async Task<EmoPicture>DetectedFaceAndExtractFace(Stream imageStream , IList<FaceAttributeType?> faceAttributes)
        {
            IList<DetectedFace> faceList = await faceClient.Face.DetectWithStreamAsync(imageStream, true, false, faceAttributes);
            EmoPicture emoPicture = new EmoPicture();
            emoPicture.Faces = ExtractFaces(faceList, emoPicture);

            return emoPicture;
        }

        private ObservableCollection<EmoFace> ExtractFaces(IList<DetectedFace> faceList, EmoPicture emoPicture)
        {
            var listFaces =new  ObservableCollection<EmoFace>();

            foreach (var face in faceList)
            {
                var emoFace = new EmoFace()
                {
                    X = face.FaceRectangle.Left,
                    Y = face.FaceRectangle.Top,
                    Width = face.FaceRectangle.Width,
                    Height = face.FaceRectangle.Height,
                    Picture = emoPicture
                };

                emoFace.Emotions = ProcessEmotions(face.FaceAttributes.Emotion, emoFace);
                listFaces.Add(emoFace);

            }
            return listFaces;

        }

        private ObservableCollection<EmoEmotion> ProcessEmotions(Emotion scores, EmoFace emoFace)
        {
            var emotionList = new ObservableCollection<EmoEmotion>();
            var properties = scores.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);


            var filterProperties = properties.Where(p => p.PropertyType == typeof(double));

            var emotype = EmoEmotionEnum.Undeterminated;

            foreach (var prop in filterProperties)
            {
                if (!Enum.TryParse<EmoEmotionEnum>(prop.Name, out emotype))
                    emotype = EmoEmotionEnum.Undeterminated;

                var emoEmotion = new EmoEmotion()
                {
                    Score = Convert.ToSingle(prop.GetValue(scores)),
                    EmotionType = emotype,
                    Face = emoFace
                };

                emotionList.Add(emoEmotion);

            }

            return emotionList;

        }
    }
}