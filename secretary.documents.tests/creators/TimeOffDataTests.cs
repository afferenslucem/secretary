using secretary.documents.creators;
using secretary.documents.templates;
using Xceed.Words.NET;

namespace secretary.documents.tests.templates
{
    public class TimeOffDataTests
    {
        public TimeOffData data;
        
        [SetUp]
        public void Setup()
        {
            this.data = new TimeOffData();
        }

        [Test]
        public void ShouldReturnDateYearForPeriod()
        {
            data.Period = "20.03.2022 с 9:00 до 13:00";

            var result = data.PeriodYear;
            
            Assert.That(result, Is.EqualTo("20.03.2022"));
        }

        [Test]
        public void ShouldReturnDateYearForOnlyYearPeriod()
        {
            data.Period = "20.03.2022";

            var result = data.PeriodYear;
            
            Assert.That(result, Is.EqualTo("20.03.2022"));
        }

        [Test]
        public void ShouldReturnNullIfPeriodNull()
        {
            data.Period = null;

            var result = data.PeriodYear;
            
            Assert.That(result, Is.EqualTo(null));
        }
    }
}