//using System;
//using System.Collections.Generic;
//using System.Net;
//using System.Net.Http;
//using System.Text;
//using System.Text.Json;
//using System.Text.Json.Nodes;
//using System.Threading;
//using System.Threading.Tasks;
//using Moq;
//using Moq.Protected;

//using Notify.Client;
//using Notify.Exceptions;
//using Notify.Interfaces;
//using Notify.Models;
//using Notify.Models.Responses;
//using NUnit.Framework;

//namespace Notify.Tests.UnitTests
//{
//    [TestFixture]
//    public class NotificationClientUnitTests
//    {
//        private Mock<HttpMessageHandler> handler;
//        private NotificationClient client;
//        public readonly JsonSerializerOptions _jsonOptions = new();

//        [SetUp]
//        public void SetUp()
//        {
//            handler = new Mock<HttpMessageHandler>();

//            var w = new HttpClientWrapper(new HttpClient(handler.Object));
//            client = new NotificationClient(w, Constants.fakeApiKey);
//        }

//        [TearDown]
//        public void TearDown()
//        {
//            handler = null;
//            client = null;
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void CreateNotificationClientWithInvalidApiKeyFails()
//        {
//            Assert.Throws<NotifyAuthException>(() => new NotificationClient("someinvalidkey"));
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void CreateNotificationClientWithEmptyApiKeyFails()
//        {
//            Assert.Throws<NotifyAuthException>(() => new NotificationClient(""));
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetNonJsonResponseHandlesException()
//        {
//            MockRequest("non json response",
//                client.GET_ALL_NOTIFICATIONS_URL,
//                AssertValidRequest, status: HttpStatusCode.NotFound);

//            var ex = Assert.Throws<NotifyClientException>(() => client.GetNotifications());
//            Assert.That(ex.Message, Does.Contain("Status code 404. Error: non json response"));
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetNotificationByIdCreatesExpectedRequest()
//        {
//            MockRequest(Constants.fakeNotificationJson,
//                client.GET_NOTIFICATION_URL + Constants.fakeNotificationId,
//                AssertValidRequest);

//            client.GetNotificationById(Constants.fakeNotificationId);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetPdfForLetterCreatesExpectedRequest()
//        {
//            var responseAsString = "%PDF-1.5 testpdf";
//            MockRequest(
//                responseAsString,
//                string.Format(client.GET_PDF_FOR_LETTER_URL, Constants.fakeNotificationId),
//                AssertValidRequest);

//            client.GetPdfForLetter(Constants.fakeNotificationId);
//        }


//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllNotificationsCreatesExpectedResult()
//        {
//            MockRequest(Constants.fakeNotificationsJson,
//                client.GET_ALL_NOTIFICATIONS_URL,
//                AssertValidRequest);

//            client.GetNotifications();
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllNotificationsWithStatusCreatesExpectedResult()
//        {
//            MockRequest(Constants.fakeNotificationsJson,
//                client.GET_ALL_NOTIFICATIONS_URL + "?status=sending",
//                AssertValidRequest);

//            client.GetNotifications(status: "sending");
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllNotificationsWithReferenceCreatesExpectedResult()
//        {
//            MockRequest(Constants.fakeNotificationsJson,
//                client.GET_ALL_NOTIFICATIONS_URL + "?reference=foo",
//                AssertValidRequest);

//            client.GetNotifications(reference: "foo");
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllNotificationsWithIncludeSpreadsheetUploadsCreatesExpectedResult()
//        {
//            MockRequest(Constants.fakeNotificationsJson,
//                client.GET_ALL_NOTIFICATIONS_URL + "?include_jobs=True",
//                AssertValidRequest);

//            client.GetNotifications(includeSpreadsheetUploads: true);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllSmsNotificationsWithStatusAndReferenceWithCreatesExpectedResult()
//        {
//            MockRequest(Constants.fakeNotificationsJson,
//                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=sms&status=sending&reference=foo",
//                AssertValidRequest);

//            client.GetNotifications(templateType: "sms", status: "sending", reference: "foo");
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllSmsNotificationsCreatesExpectedResult()
//        {
//            MockRequest(Constants.fakeSmsNotificationResponseJson,
//                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=sms",
//                AssertValidRequest);

//            client.GetNotifications(templateType: "sms");
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllEmailNotificationsCreatesExpectedResult()
//        {
//            MockRequest(Constants.fakeEmailNotificationResponseJson,
//                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=email",
//                AssertValidRequest);

//            client.GetNotifications(templateType: "email");
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllLetterNotificationsCreatesExpectedResult()
//        {
//            MockRequest(Constants.fakeEmailNotificationResponseJson,
//                client.GET_ALL_NOTIFICATIONS_URL + "?template_type=letter",
//                AssertValidRequest);

//            client.GetNotifications(templateType: "letter");
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetTemplateWithIdCreatesExpectedRequest()
//        {
//            MockRequest(Constants.fakeTemplateResponseJson,
//                client.GET_TEMPLATE_URL + Constants.fakeTemplateId,
//                AssertValidRequest);

//            client.GetTemplateByIdAndVersion(Constants.fakeTemplateId);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetTemplateWithIdAndVersionCreatesExpectedRequest()
//        {
//            MockRequest(Constants.fakeTemplateResponseJson,
//                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + client.VERSION_PARAM + "2",
//                AssertValidRequest);

//            client.GetTemplateByIdAndVersion(Constants.fakeTemplateId, 2);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetNotificationByIdReceivesExpectedResponse()
//        {
//            var expectedResponse = JsonSerializer.Deserialize<Notification>(Constants.fakeNotificationJson);

//            MockRequest(Constants.fakeNotificationJson);

//            var responseNotification = client.GetNotificationById(Constants.fakeNotificationId);
//            Assert.AreEqual(expectedResponse, responseNotification);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetPdfForLetterReceivesExpectedResponse()
//        {
//            var responseAsString = "%PDF-1.5 testpdf";
//            var expectedResponse = Encoding.UTF8.GetBytes(responseAsString);

//            MockRequest(responseAsString);

//            var actualResponse = client.GetPdfForLetter(Constants.fakeNotificationId);
//            Assert.AreEqual(expectedResponse, actualResponse);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetTemplateWithIdReceivesExpectedResponse()
//        {
//            var expectedResponse = JsonSerializer.Deserialize<TemplateResponse>(Constants.fakeTemplateResponseJson);

//            MockRequest(Constants.fakeTemplateResponseJson);

//            var responseTemplate = client.GetTemplateById(Constants.fakeTemplateId);
//            Assert.AreEqual(expectedResponse, responseTemplate);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetTemplateWithIdAndVersionReceivesExpectedResponse()
//        {
//            var expectedResponse =
//                JsonSerializer.Deserialize<TemplateResponse>(Constants.fakeTemplateResponseJson);

//            MockRequest(Constants.fakeTemplateResponseJson);

//            var responseTemplate = client.GetTemplateByIdAndVersion(Constants.fakeTemplateId, 2);
//            Assert.AreEqual(expectedResponse, responseTemplate);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GenerateTemplatePreviewGeneratesExpectedRequest()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> {
//                    { "name", "someone" }
//            };

//            var personalisationJson = new JsonObject();

//            if (personalisation != null)
//            {
//                foreach (var (key, value) in personalisation)
//                {
//                    personalisationJson.Add(key, value);
//                }
//            }

//            var o = new JsonObject
//            {
//                { "personalisation", personalisationJson }
//            };

//            MockRequest(Constants.fakeTemplatePreviewResponseJson,
//                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview", AssertValidRequest, HttpMethod.Post);

//            var response = client.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GenerateTemplatePreviewReceivesExpectedResponse()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic> {
//                    { "name", "someone" }
//            };

//            var personalisationJson = new JsonObject();

//            if (personalisation != null)
//            {
//                foreach (var (key, value) in personalisation)
//                {
//                    personalisationJson.Add(key, value);
//                }
//            }

//            var expected = new JsonObject
//            {
//                { "personalisation", personalisationJson }
//            };

//            MockRequest(Constants.fakeTemplatePreviewResponseJson,
//                client.GET_TEMPLATE_URL + Constants.fakeTemplateId + "/preview",
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent, expected.ToJsonString(_jsonOptions));

//            client.GenerateTemplatePreview(Constants.fakeTemplateId, personalisation);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllTemplatesCreatesExpectedRequest()
//        {
//            MockRequest(Constants.fakeTemplateListResponseJson,
//                 client.GET_ALL_TEMPLATES_URL, AssertValidRequest);

//            client.GetAllTemplates();
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllTemplatesBySmsTypeCreatesExpectedRequest()
//        {
//            const string type = "sms";
//            MockRequest(Constants.fakeTemplateSmsListResponseJson,
//                         client.GET_ALL_TEMPLATES_URL+ client.TYPE_PARAM + type, AssertValidRequest);

//            client.GetAllTemplates(type);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllTemplatesByEmailTypeCreatesExpectedRequest()
//        {
//            const string type = "email";

//            MockRequest(Constants.fakeTemplateEmailListResponseJson,
//                         client.GET_ALL_TEMPLATES_URL+ client.TYPE_PARAM + type, AssertValidRequest);

//            client.GetAllTemplates(type);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllTemplatesForEmptyListReceivesExpectedResponse()
//        {
//            var expectedResponse = JsonSerializer.Deserialize<TemplateList>(Constants.fakeTemplateEmptyListResponseJson);

//               MockRequest(Constants.fakeTemplateEmptyListResponseJson);

//            TemplateList templateList = client.GetAllTemplates();

//            List<TemplateResponse> templates = templateList.templates;

//            Assert.AreEqual(templates.Count, 0);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllTemplatesReceivesExpectedResponse()
//        {
//            TemplateList expectedResponse = JsonSerializer.Deserialize<TemplateList>(Constants.fakeTemplateListResponseJson);

//            MockRequest(Constants.fakeTemplateListResponseJson);

//            TemplateList templateList = client.GetAllTemplates();

//            List<TemplateResponse> templates = templateList.templates;

//            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
//            for (int i = 0; i < templates.Count; i++)
//            {
//                Assert.AreEqual(expectedResponse.templates[i], templates[i]);
//            }
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllTemplatesBySmsTypeReceivesExpectedResponse()
//        {
//            const string type = "sms";

//            TemplateList expectedResponse =
//                JsonSerializer.Deserialize<TemplateList>(Constants.fakeTemplateSmsListResponseJson);

//            MockRequest(Constants.fakeTemplateSmsListResponseJson,
//                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

//            TemplateList templateList = client.GetAllTemplates(type);

//            List<TemplateResponse> templates = templateList.templates;

//            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
//            for (int i = 0; i < templates.Count; i++)
//            {
//                Assert.AreEqual(expectedResponse.templates[i], templates[i]);
//            }
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllTemplatesByEmailTypeReceivesExpectedResponse()
//        {
//            const string type = "email";

//            TemplateList expectedResponse =
//                JsonSerializer.Deserialize<TemplateList>(Constants.fakeTemplateEmailListResponseJson);

//            MockRequest(Constants.fakeTemplateEmailListResponseJson,
//                         client.GET_ALL_TEMPLATES_URL + client.TYPE_PARAM + type, AssertValidRequest);

//            TemplateList templateList = client.GetAllTemplates(type);

//            List<TemplateResponse> templates = templateList.templates;

//            Assert.AreEqual(templates.Count, expectedResponse.templates.Count);
//            for (int i = 0; i < templates.Count; i++)
//            {
//                Assert.AreEqual(expectedResponse.templates[i], templates[i]);
//            }
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllReceivedTextsCreatesExpectedRequest()
//        {
//            MockRequest(Constants.fakeReceivedTextListResponseJson,
//                 client.GET_RECEIVED_TEXTS_URL, AssertValidRequest);

//            client.GetReceivedTexts();
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void GetAllReceivedTextsReceivesExpectedResponse()
//        {
//            MockRequest(Constants.fakeReceivedTextListResponseJson,
//                 client.GET_RECEIVED_TEXTS_URL, AssertValidRequest);

//            ReceivedTextListResponse expectedResponse =
//                JsonSerializer.Deserialize<ReceivedTextListResponse>(Constants.fakeReceivedTextListResponseJson);

//            MockRequest(Constants.fakeReceivedTextListResponseJson,
//                         client.GET_RECEIVED_TEXTS_URL, AssertValidRequest);

//            ReceivedTextListResponse receivedTextList = client.GetReceivedTexts();

//            List<ReceivedTextResponse> receivedTexts = receivedTextList.receivedTexts;

//            Assert.AreEqual(receivedTexts.Count, expectedResponse.receivedTexts.Count);
//            for (int i = 0; i < receivedTexts.Count; i++)
//            {
//                Assert.AreEqual(expectedResponse.receivedTexts[i], receivedTexts[i]);
//            }
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendSmsNotificationGeneratesExpectedRequest()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
//                {
//                    { "name", "someone" }
//                };

//            var personalisationJson = new JsonObject();

//            if (personalisation != null)
//            {
//                foreach (var (key, value) in personalisation)
//                {
//                    personalisationJson.Add(key, value);
//                }
//            }

//            JsonObject expected = new JsonObject
//            {
//                { "phone_number", Constants.fakePhoneNumber },
//                { "template_id", Constants.fakeTemplateId },
//                { "personalisation", personalisationJson }
//            };

//            MockRequest(Constants.fakeSmsNotificationResponseJson,
//                client.SEND_SMS_NOTIFICATION_URL,
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent, expected.ToJsonString(_jsonOptions));

//            SmsNotificationResponse response = client.SendSms(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendSmsNotificationGeneratesExpectedResponse()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
//                {
//                    { "name", "someone" }
//                };
//            SmsNotificationResponse expectedResponse = JsonSerializer.Deserialize<SmsNotificationResponse>(Constants.fakeSmsNotificationResponseJson);

//            MockRequest(Constants.fakeSmsNotificationResponseJson);

//            SmsNotificationResponse actualResponse = client.SendSms(Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation);

//            Assert.AreEqual(expectedResponse, actualResponse);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendEmailNotificationGeneratesExpectedRequest()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
//                {
//                    { "name", "someone" }
//                };

//            var personalisationJson = new JsonObject();

//            if (personalisation != null)
//            {
//                foreach (var (key, value) in personalisation)
//                {
//                    personalisationJson.Add(key, value);
//                }
//            }

//            JsonObject expected = new JsonObject
//            {
//                { "email_address", Constants.fakeEmail },
//                { "template_id", Constants.fakeTemplateId },
//                { "personalisation", personalisationJson },
//                { "reference", Constants.fakeNotificationReference }
//            };

//            MockRequest(Constants.fakeTemplatePreviewResponseJson,
//                client.SEND_EMAIL_NOTIFICATION_URL,
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent, expected.ToJsonString(_jsonOptions));

//            EmailNotificationResponse response = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendEmailNotificationWithDocumentGeneratesExpectedRequest()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
//                {
//                    { "document", NotificationClient.PrepareUpload(Encoding.UTF8.GetBytes("%PDF-1.5 testpdf")) }
//                };
//            JsonObject expected = new JsonObject
//            {
//                { "email_address", Constants.fakeEmail },
//                { "template_id", Constants.fakeTemplateId },
//                { "personalisation", new JsonObject
//                  {
//                    {"document", new JsonObject
//                      {
//                        {"file", "JVBERi0xLjUgdGVzdHBkZg=="},
//                        {"is_csv", false}
//                      }
//                    }
//                  }
//                },
//                { "reference", Constants.fakeNotificationReference }
//            };

//            MockRequest(Constants.fakeTemplatePreviewResponseJson,
//                client.SEND_EMAIL_NOTIFICATION_URL,
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent, expected.ToJsonString(_jsonOptions));

//            EmailNotificationResponse response = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendEmailNotificationWithCSVDocumentGeneratesExpectedRequest()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
//                {
//                    { "document", NotificationClient.PrepareUpload(Encoding.UTF8.GetBytes("%PDF-1.5 testpdf"), true) }
//                };
//            JsonObject expected = new JsonObject
//            {
//                { "email_address", Constants.fakeEmail },
//                { "template_id", Constants.fakeTemplateId },
//                { "personalisation", new JsonObject
//                  {
//                    {"document", new JsonObject
//                      {
//                        {"file", "JVBERi0xLjUgdGVzdHBkZg=="},
//                        {"is_csv", true}
//                      }
//                    }
//                  }
//                },
//                { "reference", Constants.fakeNotificationReference }
//            };

//            MockRequest(Constants.fakeTemplatePreviewResponseJson,
//                client.SEND_EMAIL_NOTIFICATION_URL,
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent, expected.ToJsonString(_jsonOptions));

//            EmailNotificationResponse response = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void PrepareUploadWithLargeDocumentGeneratesAnError()
//        {
//            Assert.That(
//                    () => { NotificationClient.PrepareUpload(new byte[3*1024*1024]); },
//                    Throws.ArgumentException
//                    );
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendEmailNotificationGeneratesExpectedResponse()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
//                {
//                    { "name", "someone" }
//                };
//            EmailNotificationResponse expectedResponse = JsonSerializer.Deserialize<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

//            MockRequest(Constants.fakeEmailNotificationResponseJson);

//            EmailNotificationResponse actualResponse = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

//            Assert.AreEqual(expectedResponse, actualResponse);

//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendLetterNotificationGeneratesExpectedRequest()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
//                {
//                    { "address_line_1", "Foo" },
//                    { "address_line_2", "Bar" },
//                    { "postcode", "SW1 1AA" }
//                };

//            var personalisationJson = new JsonObject();

//            if (personalisation != null)
//            {
//                foreach (var (key, value) in personalisation)
//                {
//                    personalisationJson.Add(key, value);
//                }
//            }

//            JsonObject expected = new JsonObject
//            {
//                { "template_id", Constants.fakeTemplateId },
//                { "personalisation", personalisationJson },
//                { "reference", Constants.fakeNotificationReference }
//            };

//            MockRequest(Constants.fakeTemplatePreviewResponseJson,
//                client.SEND_LETTER_NOTIFICATION_URL,
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent, expected.ToJsonString(_jsonOptions));

//            LetterNotificationResponse response = client.SendLetter(Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendLetterNotificationGeneratesExpectedResponse()
//        {
//            Dictionary<string, dynamic> personalisation = new Dictionary<string, dynamic>
//                {
//                    { "address_line_1", "Foo" },
//                    { "address_line_2", "Bar" },
//                    { "postcode", "SW1 1AA" }
//                };
//            LetterNotificationResponse expectedResponse = JsonSerializer.Deserialize<LetterNotificationResponse>(Constants.fakeLetterNotificationResponseJson);

//            MockRequest(Constants.fakeLetterNotificationResponseJson);

//            LetterNotificationResponse actualResponse = client.SendLetter(Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference);

//            Assert.AreEqual(expectedResponse, actualResponse);

//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendPrecompiledLetterNotificationGeneratesExpectedRequest()
//        {
//            JsonObject expected = new JsonObject
//            {
//                { "reference", Constants.fakeNotificationReference },
//                { "content", "JVBERi0xLjUgdGVzdHBkZg==" }
//            };

//            MockRequest(Constants.fakeTemplatePreviewResponseJson,
//                client.SEND_LETTER_NOTIFICATION_URL,
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent, expected.ToJsonString(_jsonOptions));

//            LetterNotificationResponse response = client.SendPrecompiledLetter(
//                    Constants.fakeNotificationReference,
//                    Encoding.UTF8.GetBytes("%PDF-1.5 testpdf")
//            );
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendPrecompiledLetterNotificationGeneratesExpectedRequestWithPostage()
//        {
//            JsonObject expected = new JsonObject
//            {
//                { "reference", Constants.fakeNotificationReference },
//                { "content", "JVBERi0xLjUgdGVzdHBkZg==" },
//                { "postage", "first" }
//            };

//            MockRequest(Constants.fakeTemplatePreviewResponseJson,
//                client.SEND_LETTER_NOTIFICATION_URL,
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent, expected.ToJsonString(_jsonOptions));

//            LetterNotificationResponse response = client.SendPrecompiledLetter(
//                    Constants.fakeNotificationReference,
//                    Encoding.UTF8.GetBytes("%PDF-1.5 testpdf"),
//                    "first"
//            );
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendPrecompiledLetterNotificationGeneratesExpectedResponse()
//        {
//            LetterNotificationResponse expectedResponse = JsonSerializer.Deserialize<LetterNotificationResponse>(Constants.fakePrecompiledLetterNotificationResponseJson);

//            MockRequest(Constants.fakePrecompiledLetterNotificationResponseJson);

//            LetterNotificationResponse actualResponse = client.SendPrecompiledLetter(Constants.fakeNotificationReference, Encoding.UTF8.GetBytes("%PDF-1.5 testpdf"));

//            Assert.IsNotNull(expectedResponse.id);
//            Assert.IsNotNull(expectedResponse.reference);
//            Assert.IsNotNull(expectedResponse.postage);
//            Assert.IsNull(expectedResponse.content);
//            Assert.AreEqual(expectedResponse, actualResponse);

//        }

//        private static void AssertGetExpectedContent(string expected, string content)
//        {
//            Assert.IsNotNull(content);
//            Assert.AreEqual(expected, content);
//        }

//        private void AssertValidRequest(string uri, HttpRequestMessage r, HttpMethod method = null)
//        {
//            if (method == null)
//            {
//                method = HttpMethod.Get;
//            }

//            Assert.AreEqual(r.Method, method);
//            Assert.AreEqual(r.RequestUri.ToString(), client.BaseUrl + uri);
//            Assert.IsNotNull(r.Headers.Authorization);
//            Assert.IsNotNull(r.Headers.UserAgent);
//            Assert.AreEqual(r.Headers.UserAgent.ToString(), client.GetUserAgent());
//            Assert.AreEqual(r.Headers.Accept.ToString(), "application/json");
//        }

//        private void MockRequest(string content, string uri,
//                          Action<string, HttpRequestMessage, HttpMethod> _assertValidRequest = null,
//                          HttpMethod method = null,
//                          Action<string, string> _assertGetExpectedContent = null,
//                          string expected = null,
//                          HttpStatusCode status = HttpStatusCode.OK)
//        {
//            handler.Protected()
//                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage
//                {
//                    StatusCode = status,
//                    Content = new StringContent(content)
//                }))
//                .Callback<HttpRequestMessage, CancellationToken>((r, c) =>
//                {
//                    _assertValidRequest(uri, r, method);

//                    if (r.Content == null || _assertGetExpectedContent == null) return;

//                    var response = r.Content.ReadAsStringAsync().Result;
//                    _assertGetExpectedContent(expected, response);
//                });
//        }

//        private void MockRequest(string content)
//        {

//            handler.Protected()
//                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
//                .Returns(Task<HttpResponseMessage>.Factory.StartNew(() => new HttpResponseMessage
//                {
//                    StatusCode = HttpStatusCode.OK,
//                    Content = new StringContent(content)
//                }));
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendEmailNotificationWithReplyToIdGeneratesExpectedRequest()
//        {
//            var personalisation = new Dictionary<string, dynamic>
//            {
//                { "name", "someone" }
//            };

//            var personalisationJson = new JsonObject();

//            if (personalisation != null)
//            {
//                foreach (var (key, value) in personalisation)
//                {
//                    personalisationJson.Add(key, value);
//                }
//            }

//            var expected = new JsonObject
//            {
//                { "email_address", Constants.fakeEmail },
//                { "template_id", Constants.fakeTemplateId },
//                { "personalisation", personalisationJson },
//                { "reference", Constants.fakeNotificationReference },
//                { "email_reply_to_id", Constants.fakeReplyToId}
//            };

//            MockRequest(Constants.fakeTemplateEmailListResponseJson,
//                client.SEND_EMAIL_NOTIFICATION_URL,
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent,
//                expected.ToJsonString(_jsonOptions));

//            var response = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference, Constants.fakeReplyToId);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendEmailNotificationWithReplyToIdGeneratesExpectedResponse()
//        {
//            var personalisation = new Dictionary<string, dynamic>
//            {
//                { "name", "someone" }
//            };

//            var expectedResponse = JsonSerializer.Deserialize<EmailNotificationResponse>(Constants.fakeEmailNotificationResponseJson);

//            MockRequest(Constants.fakeEmailNotificationResponseJson);

//            var actualResponse = client.SendEmail(Constants.fakeEmail, Constants.fakeTemplateId, personalisation, Constants.fakeNotificationReference, Constants.fakeReplyToId);

//            Assert.AreEqual(expectedResponse, actualResponse);
//        }

//        [Test, Category("Unit"), Category("Unit/NotificationClient")]
//        public void SendSmsNotificationWithSmsSenderIdGeneratesExpectedRequest()
//        {
//            var personalisation = new Dictionary<string, dynamic>
//                {
//                    { "name", "someone" }
//                };

//            var personalisationJson = new JsonObject();

//            if (personalisation != null)
//            {
//                foreach (var (key, value) in personalisation)
//                {
//                    personalisationJson.Add(key, value);
//                }
//            }

//            var expected = new JsonObject
//            {
//                { "phone_number", Constants.fakePhoneNumber },
//                { "template_id", Constants.fakeTemplateId },
//                { "personalisation", personalisationJson },
//                { "sms_sender_id", Constants.fakeSMSSenderId }
//            };

//            MockRequest(Constants.fakeSmsNotificationWithSMSSenderIdResponseJson,
//                client.SEND_SMS_NOTIFICATION_URL,
//                AssertValidRequest,
//                HttpMethod.Post,
//                AssertGetExpectedContent, expected.ToJsonString(_jsonOptions));

//            var response = client.SendSms(
//                Constants.fakePhoneNumber, Constants.fakeTemplateId, personalisation: personalisation, smsSenderId: Constants.fakeSMSSenderId);
//        }
//    }
//}
