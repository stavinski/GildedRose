using System.Collections.Generic;
using GildedRose.Console;
using Xunit;

namespace GildedRose.Tests
{
    public class EndOfDayTests
    {
        [Fact]
        public void 
        Existing_stock_behaves_the_same()
        {
            var endOfDay = new EndOfDay(new List<Item>
                                          {
                                              new Item {Name = "+5 Dexterity Vest", SellIn = 10, Quality = 20},
                                              new Item {Name = "Aged Brie", SellIn = 2, Quality = 0},
                                              new Item {Name = "Elixir of the Mongoose", SellIn = 5, Quality = 7},
                                              new Item {Name = "Sulfuras, Hand of Ragnaros", SellIn = 0, Quality = 80},
                                              new Item
                                                  {
                                                      Name = "Backstage passes to a TAFKAL80ETC concert",
                                                      SellIn = 15,
                                                      Quality = 20
                                                  },
                                              new Item {Name = "Conjured Mana Cake", SellIn = 3, Quality = 6}
                                          });

            endOfDay.UpdateQuality();
            var expected = endOfDay.Stock;

            Assert.Equal(19, expected[0].Quality);
            Assert.Equal(9, expected[0].SellIn);

            Assert.Equal(1, expected[1].Quality);
            Assert.Equal(1, expected[1].SellIn);

            Assert.Equal(6, expected[2].Quality);
            Assert.Equal(4, expected[2].SellIn);

            Assert.Equal(80, expected[3].Quality);
            Assert.Equal(0, expected[3].SellIn);

            Assert.Equal(21, expected[4].Quality);
            Assert.Equal(14, expected[4].SellIn);

            Assert.Equal(5, expected[5].Quality);
            Assert.Equal(2, expected[5].SellIn);
        }

        [Fact]
        public void
        Once_sell_passed_quality_degrades_twice_as_fast()
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = "Elixir of the Mongoose", SellIn = 0, Quality = 2 }
            };
            
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(0, stock[0].Quality);
        }

        [Fact]
        public void
        Quality_never_goes_negative()
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = "Elixir of the Mongoose", SellIn = 1, Quality = 0 }
            };

            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(0, stock[0].Quality);
        }

        [Fact]
        public void
        Aged_brie_increases_in_quality_the_older_it_gets()
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = EndOfDay.AGED_BRIE, SellIn = 1, Quality = 0 }
            };

            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(1, stock[0].Quality);
        }

        [Fact]
        public void
        Quality_never_goes_above_50()
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = EndOfDay.AGED_BRIE, SellIn = 1, Quality = 50 }
            };

            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(50, stock[0].Quality);
        }

        [Fact]
        public void
        Sulfuras_do_not_decrease_in_quality()
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = EndOfDay.SULFURAS, SellIn = 1, Quality = 1 }
            };

            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(1, stock[0].Quality);
        }

        [Fact]
        public void
        Sulfuras_do_not_decrease_in_sell_in()
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = EndOfDay.SULFURAS, SellIn = 1, Quality = 1 }
            };

            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(1, stock[0].SellIn);
        }

        [Fact]
        public void
        Backstage_passes_increase_in_quality_by_1_over_10_days()
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = EndOfDay.BACKSTAGE_PASSES, SellIn = 11, Quality = 0 }
            };

            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(1, stock[0].Quality);
        }

        [Theory]
        [InlineData(6)]
        [InlineData(7)]
        [InlineData(8)]
        [InlineData(9)]
        [InlineData(10)]
        public void
        Backstage_passes_increase_in_quality_by_2_between_6_to_10_days(int days)
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = EndOfDay.BACKSTAGE_PASSES, SellIn = days, Quality = 0 }
            };

            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(2, stock[0].Quality);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        [InlineData(5)]
        public void
        Backstage_passes_increase_in_quality_by_3_between_5_to_1_days(int days)
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = EndOfDay.BACKSTAGE_PASSES, SellIn = days, Quality = 0 }
            };

            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(3, stock[0].Quality);
        }

        public void
        Backstage_passes_quality_is_zero_after_concert()
        {
            IList<Item> stock = new List<Item>
            {
                new Item { Name = EndOfDay.BACKSTAGE_PASSES, SellIn = 0, Quality = 3 }
            };

            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            Assert.Equal(0, stock[0].Quality);
        }
    }
}