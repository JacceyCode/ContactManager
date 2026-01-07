namespace ContactManagerTest
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            // Arrange
            Math math = new();
            int a = 10, b = 5;

            // Act
            int result = math.Add(a, b);

            // Assert
            Assert.Equal(15, result);
        }
    }
}
