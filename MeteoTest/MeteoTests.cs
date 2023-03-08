using MeteoProj;

namespace MeteoTest
{
    public class MeteoTests // These tests do not necessarily have to be passed successfully
    {
        [Fact]
        public void Test1()
        {
            Program.MessageHandling("$15");
            Program.MessageHandling(".16");
            Program.MessageHandling(",");
            Program.MessageHandling("158.3");

            Assert.Equal("$15.16,158.3", Program.RecMsg);
            Program.RecMsg = "";
        }

        [Fact]
        public void Test2()
        {
            Program.MessageHandling("$11.");
            Program.MessageHandling("12");
            Program.MessageHandling(",18.77");

            Assert.Equal("$11.12,18.77", Program.RecMsg);
            Program.RecMsg = "";
        }

        [Fact]
        public void Test3()
        {
            Program.MessageHandling("$");
            Program.MessageHandling("1");
            Program.MessageHandling("2");
            Program.MessageHandling("3");
            Program.MessageHandling(".");
            Program.MessageHandling("4");
            Program.MessageHandling("5");
            Program.MessageHandling(",");
            Program.MessageHandling("6");
            Program.MessageHandling("7");
            Program.MessageHandling(".");
            Program.MessageHandling("8");

            Assert.Equal("$123.45,67.8", Program.RecMsg);
            Program.RecMsg = "";
        }

        [Fact]
        public void Test4()
        {
            Program.MessageHandling("%!¹;%:$77.");
            Program.MessageHandling("33");
            Program.MessageHandling(",44.77+_)(*?:");

            Assert.Equal("$11.33,44.77", Program.RecMsg);
            Program.RecMsg = "";
        }

        [Fact]
        public void Test5()
        {
            Program.MessageHandling("");

            Assert.Equal("", Program.RecMsg);
            Program.RecMsg = "";
        }

        [Fact]
        public void Test6()
        {
            Program.MessageHandling("$11.33,44.77$123.45,67.8");

            Assert.Equal("", Program.RecMsg);
            Program.RecMsg = "";
        }

        [Fact]
        public void Test7()
        {
            Program.MessageHandling("$11.33,44.77");

            Assert.Equal("$11.33,44.77", Program.RecMsg);
            Program.RecMsg = "";
        }

        [Fact]
        public void Test9()
        {
            Program.MessageHandling("$22.33,44.77$11.33,44.77$12.42,");
            Program.MessageHandling("15.22");

            Assert.Equal("$11.33,44.77", Program.RecMsg);
            Program.RecMsg = "";
        }

        [Fact]
        public void Test10()
        {
            Program.MessageHandling("$11.33,44.77$11.33,44.77");
            Program.MessageHandling("15.22");

            Assert.Equal("$11.33,44.77", Program.RecMsg);
            Program.RecMsg = "";
        }

        [Fact]
        public void Test11()
        {
            Program.MessageHandling("$11.11,11.11$22.22,22.22$44.44,44.44");

            Assert.Equal("$11.33,44.77", Program.RecMsg);
            Program.RecMsg = "";
        }
    }
}