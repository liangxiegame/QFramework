using NUnit.Framework;

namespace QFramework.Tests
{
    public class EasyGridTests
    {
        [Test]
        public void Constructor_Sets_Width_Height()
        {
            var grid = new EasyGrid<string>(4, 4);

            Assert.AreEqual(4, grid.Width);
            Assert.AreEqual(4, grid.Height);
        }

        [Test]
        public void Fill_Sets_All_Cells()
        {
            var grid = new EasyGrid<string>(3, 2);

            grid.Fill("x");

            for (var x = 0; x < grid.Width; x++)
            {
                for (var y = 0; y < grid.Height; y++)
                {
                    Assert.AreEqual("x", grid[x, y]);
                }
            }
        }

        [Test]
        public void Fill_With_Func()
        {
            var grid = new EasyGrid<string>(3, 2);

            grid.Fill((x, y) => $"{x},{y}");

            Assert.AreEqual("0,0", grid[0, 0]);
            Assert.AreEqual("0,1", grid[0, 1]);
            Assert.AreEqual("1,0", grid[1, 0]);
            Assert.AreEqual("1,1", grid[1, 1]);
            Assert.AreEqual("2,0", grid[2, 0]);
            Assert.AreEqual("2,1", grid[2, 1]);
        }

        [Test]
        public void Indexer_Get_Set()
        {
            var grid = new EasyGrid<string>(4, 4);

            grid[2, 3] = "hello";

            Assert.AreEqual("hello", grid[2, 3]);
        }

        [Test]
        public void Resize_Expands_Grid()
        {
            var grid = new EasyGrid<string>(4, 4);
            grid[0, 0] = "origin";
            grid[3, 3] = "corner";

            grid.Resize(6, 6, (x, y) => $"added:{x},{y}");

            Assert.AreEqual(6, grid.Width);
            Assert.AreEqual(6, grid.Height);
            Assert.AreEqual("origin", grid[0, 0]);
            Assert.AreEqual("corner", grid[3, 3]);
            Assert.AreEqual("added:4,0", grid[4, 0]);
            Assert.AreEqual("added:5,5", grid[5, 5]);
            Assert.AreEqual("added:0,4", grid[0, 4]);
            Assert.AreEqual("added:3,5", grid[3, 5]);
        }

        [Test]
        public void ForEach_Visits_All_Cells()
        {
            var grid = new EasyGrid<string>(3, 2);
            var visits = 0;
            var sawLastCell = false;

            grid.ForEach((x, y, value) =>
            {
                visits++;
                sawLastCell |= x == 2 && y == 1;
            });

            Assert.AreEqual(6, visits);
            Assert.IsTrue(sawLastCell);
        }

        [Test]
        public void ForEach_Single_Arg()
        {
            var grid = new EasyGrid<string>(2, 2);
            grid.Fill("x");
            var visits = 0;

            grid.ForEach(value =>
            {
                visits++;
                Assert.AreEqual("x", value);
            });

            Assert.AreEqual(4, visits);
        }

        [Test]
        public void Clear_Nullifies_Grid()
        {
            var grid = new EasyGrid<string>(2, 2);
            grid.Fill("x");

            grid.Clear();

            Assert.AreEqual(2, grid.Width);
            Assert.AreEqual(2, grid.Height);
            Assert.Throws<System.NullReferenceException>(() => grid.ForEach(value => { }));
        }
    }

    public class DynaGridTests
    {
        [Test]
        public void Indexer_Get_Returns_Default_When_Empty()
        {
            var grid = new DynaGrid<string>();

            Assert.IsNull(grid[0, 0]);
        }

        [Test]
        public void Indexer_Set_Then_Get()
        {
            var grid = new DynaGrid<string>();

            grid[2, 3] = "hello";

            Assert.AreEqual("hello", grid[2, 3]);
        }

        [Test]
        public void Negative_Indices_Work()
        {
            var grid = new DynaGrid<string>();

            grid[-1, -10] = "x";

            Assert.AreEqual("x", grid[-1, -10]);
        }

        [Test]
        public void ForEach_Visits_All_Cells()
        {
            var grid = new DynaGrid<string>();
            grid[0, 0] = "origin";
            grid[5, 6] = "far";
            grid[-1, -10] = "negative";
            var visits = 0;
            var sawOrigin = false;
            var sawFar = false;
            var sawNegative = false;

            grid.ForEach((x, y, value) =>
            {
                visits++;
                sawOrigin |= x == 0 && y == 0 && value == "origin";
                sawFar |= x == 5 && y == 6 && value == "far";
                sawNegative |= x == -1 && y == -10 && value == "negative";
            });

            Assert.AreEqual(3, visits);
            Assert.IsTrue(sawOrigin);
            Assert.IsTrue(sawFar);
            Assert.IsTrue(sawNegative);
        }

        [Test]
        public void ForEach_Single_Arg()
        {
            var grid = new DynaGrid<string>();
            grid[0, 0] = "origin";
            grid[5, 6] = "far";
            var visits = 0;
            var sawOrigin = false;
            var sawFar = false;

            grid.ForEach(value =>
            {
                visits++;
                sawOrigin |= value == "origin";
                sawFar |= value == "far";
            });

            Assert.AreEqual(2, visits);
            Assert.IsTrue(sawOrigin);
            Assert.IsTrue(sawFar);
        }

        [Test]
        public void Clear_Empties_Grid()
        {
            var grid = new DynaGrid<string>();
            grid[0, 0] = "origin";
            grid[5, 6] = "far";

            grid.Clear();

            var visits = 0;
            grid.ForEach(value => visits++);
            Assert.AreEqual(0, visits);
            Assert.IsNull(grid[0, 0]);
            Assert.IsNull(grid[5, 6]);
        }
    }
}
