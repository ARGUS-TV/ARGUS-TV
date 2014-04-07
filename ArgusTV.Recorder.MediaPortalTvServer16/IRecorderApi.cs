using ArgusTV.DataContracts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Web;
using System.Text;

namespace ArgusTV.Recorder.MediaPortalTvServer
{
    [ServiceContract]
    public interface IRecorderApi
    {
        [OperationContract]
        [WebGet(UriTemplate = "/Ping")]
        Stream Ping();

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/KeepAlive")]
        void KeepAlive();

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Initialize/{recorderId}", RequestFormat = WebMessageFormat.Json)]
        void Initialize(string recorderId, Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/AllocateCard", RequestFormat = WebMessageFormat.Json)]
        Stream AllocateCard(Stream body);

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Recording/Start", RequestFormat = WebMessageFormat.Json)]
        Stream RecordingStart(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Recording/ValidateAndUpdate", RequestFormat = WebMessageFormat.Json)]
        Stream RecordingValidateAndUpdate(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Recording/Abort", RequestFormat = WebMessageFormat.Json)]
        Stream RecordingAbort(Stream body);

        [OperationContract]
        [WebGet(UriTemplate = "/RecordingShares")]
        Stream RecordingShares();

        [OperationContract]
        [WebGet(UriTemplate = "/TimeshiftShares")]
        Stream TimeshiftShares();

        [OperationContract]
        [WebInvoke(Method = "POST", UriTemplate = "/Live/Tune", RequestFormat = WebMessageFormat.Json)]
        Stream LiveTune(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Live/KeepAlive", RequestFormat = WebMessageFormat.Json)]
        Stream LiveKeepAlive(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Live/Stop", RequestFormat = WebMessageFormat.Json)]
        void LiveStop(Stream body);

        [OperationContract]
        [WebGet(UriTemplate = "/LiveStreams")]
        Stream LiveStreams();

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/ChannelsLiveState", RequestFormat = WebMessageFormat.Json)]
        Stream ChannelsLiveState(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Live/TuningDetails", RequestFormat = WebMessageFormat.Json)]
        Stream LiveTuningDetails(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Live/HasTeletext", RequestFormat = WebMessageFormat.Json)]
        Stream LiveHasTeletext(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Live/Teletext/StartGrabbing", RequestFormat = WebMessageFormat.Json)]
        void LiveTeletextStartGrabbing(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Live/Teletext/StopGrabbing", RequestFormat = WebMessageFormat.Json)]
        void LiveTeletextStopGrabbing(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Live/Teletext/IsGrabbing", RequestFormat = WebMessageFormat.Json)]
        Stream LiveTeletextIsGrabbing(Stream body);

        [OperationContract]
        [WebInvoke(Method = "PUT", UriTemplate = "/Live/Teletext/GetPage/{pageNumber}/{subPageNumber}", RequestFormat = WebMessageFormat.Json)]
        Stream LiveTeletextGetPage(string pageNumber, string subPageNumber, Stream body);
    }
}
