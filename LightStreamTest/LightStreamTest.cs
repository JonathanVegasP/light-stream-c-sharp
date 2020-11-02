using System.Threading;
using LightStream.Exceptions;
using LightStream.Extensions;
using LightStream.Interfaces;
using Xunit;

namespace LightStreamTest
{
    public class LightStreamTest
    {
        // Initialize resources
        private readonly ManualResetEvent re = new ManualResetEvent(false);

        private readonly ILightStream<string> stream = "".LS();

        [Fact(DisplayName = "Test initialized resources")]
        public void TestInitializedResource()
        {
            // Check if the initial value is working
            Assert.NotNull(stream.Value);

            // Check if the stream has a Sink (For only entry data purpose)
            var sink = stream.Sink;
            Assert.NotNull(sink);
        }

        [Fact(DisplayName = "Test the LightStream subscription")]
        public void TestStreamSubscription()
        {
            // Initialize resources
            var sink = stream.Sink;

            // Create a subscription and listen when the stream receive a new event
            var value = "";
            object valueError = null;
            var subscription = stream.Listen((data) =>
            {
                value = $"Event: {data}";
                re.Set();
            }, onError: (error) =>
            {
                valueError = $"Error: {error}";
                re.Set();
            });

            // Stream and Sink must have a listener after the listen method was triggered;
            Assert.True(stream.HasListeners);
            Assert.True(sink.HasListeners);

            // Stream and Sink must have only one listener as the listen was trigger only
            // one time
            Assert.Equal(1, stream.Length);
            Assert.Equal(1, sink.Length);

            // Check if the listen returns a Subscription to control how the Stream trigger
            // an action after a new event
            Assert.NotNull(subscription);

            // Check if the listen returned is resumed
            Assert.False(subscription.IsPaused);

            // Trigger the listen with a new event at the Stream and check the new value
            stream.Add("Hello Stream");
            Assert.Equal("Hello Stream", stream.Value);
            re.WaitOne();
            Assert.Equal("Event: Hello Stream", value);
            re.Reset();

            // Trigger the listen with a new event at the Sink and check the new value
            sink.Add("Hello Sink");
            Assert.Equal("Hello Sink", stream.Value);
            re.WaitOne();
            Assert.Equal("Event: Hello Sink", value);
            re.Reset();

            // Trigger the listen with a new error at the Stream
            stream.AddError("Something went wrong");
            re.WaitOne();
            Assert.Equal("Error: Something went wrong", valueError);
            re.Reset();
            valueError = null;

            // Trigger the listen with a new error at the Sink
            sink.AddError("Something went wrong");
            re.WaitOne();
            Assert.Equal("Error: Something went wrong", valueError);
            re.Reset();

            // Change the event callback inside the subscription
            subscription.OnEvent((data) =>
            {
                value = data;
                re.Set();
            });

            // Check if the new event will be displayed without Event tag
            stream.Add("Streams are awesome");
            re.WaitOne();
            Assert.Equal("Streams are awesome", value);
            re.Reset();

            // Change the error callback inside the subscription
            subscription.OnError((error) =>
            {
                valueError = error;
                re.Set();
            });

            // Check if the new error will be displayed without an Error tag
            stream.AddError("Reactive programming is awesome");
            re.WaitOne();
            Assert.Equal("Reactive programming is awesome", valueError);
            re.Reset();

            // Check when the subscription is paused if it does not trigger any callback
            subscription.Pause();
            stream.Add("Event is paused");
            re.WaitOne(1000);
            Assert.NotEqual("Event is paused", value);
            Assert.True(subscription.IsPaused);
            re.Reset();

            //Check when the subscription is resumed if it trigger an Event callback;
            subscription.Resume();
            stream.Add("Event is resumed");
            re.WaitOne();
            Assert.Equal("Event is resumed", value);
            Assert.False(subscription.IsPaused);
            re.Reset();

            // Check if the subscription can be unsubscribed without Stream or Sink
            subscription.Cancel();
            Assert.False(stream.HasListeners);
            Assert.Equal(0, stream.Length);
        }

        [Fact(DisplayName = "Test the LightStream Close")]
        public async void TestLightStream()
        {
            //Initialize resources
            var sink = stream.Sink;
            var value = "";

            // Add a new subscription with only done callback
            var subscription = stream.Listen(null, onDone: () =>
             {
                 value = "Done";
                 re.Set();
             });
            Assert.True(stream.HasListeners);
            Assert.Equal(1, stream.Length);

            // Close all listeners and trigger done callback
            await stream.Close(); //Or await sink.Close(); as the Stream use the same method as Sink
            re.WaitOne();
            Assert.Equal("Done", value);
            Assert.Null(stream.Value);
            Assert.True(stream.IsClosed);
            Assert.True(sink.IsClosed);
            Assert.False(stream.HasListeners);
            Assert.False(sink.HasListeners);
            Assert.Equal(0, stream.Length);
            Assert.Equal(0, sink.Length);

            // Check when adding a new event to a closed Stream an error has to be occured
            Assert.Throws<LightStreamException>(() => stream.Add(null));

            // Check when adding a new event to a closed Sink an error has to be occured
            Assert.Throws<LightStreamException>(() => sink.Add(null));

            // Check when listening to a closed Stream an error has to be occured
            Assert.Throws<LightStreamException>(() => stream.Listen(null));

            // Check when closing a closed Stream an error has to be occured
            await Assert.ThrowsAsync<LightStreamException>(() => stream.Close());

            //Check when closing a closed Sink an error has to be occured
            await Assert.ThrowsAsync<LightStreamException>(() => sink.Close());
        }
    }
}
