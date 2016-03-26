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
            var stock = CreateSingleStockItemWith(name: "Elixir of the Mongoose", sellIn: 0, quality: 2);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            var actual = endOfDay.Stock[0];
            Assert.Equal(0, actual.Quality);
        }

        [Fact]
        public void
        Quality_never_goes_negative()
        {
            var stock = CreateSingleStockItemWith(name: "Elixir of the Mongoose", sellIn: 1, quality: 0);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            var actual = endOfDay.Stock[0];
            Assert.Equal(0, actual.Quality);
        }

        [Fact]
        public void
        Aged_brie_increases_in_quality_the_older_it_gets()
        {
            var stock = CreateSingleStockItemWith(name: EndOfDay.AGED_BRIE, sellIn: 1, quality: 0);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            var actual = endOfDay.Stock[0];
            Assert.Equal(1, actual.Quality);
        }

        [Fact]
        public void
        Quality_never_goes_above_50()
        {
            var stock = CreateSingleStockItemWith(name: EndOfDay.AGED_BRIE, sellIn: 1, quality: 50);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            var actual = endOfDay.Stock[0];
            Assert.Equal(50, actual.Quality);
        }

        [Fact]
        public void
        Sulfuras_do_not_decrease_in_quality()
        {
            var stock = CreateSingleStockItemWith(name: EndOfDay.SULFURAS, sellIn: 1, quality: 1);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            var actual = endOfDay.Stock[0];
            Assert.Equal(1, actual.Quality);
        }

        [Fact]
        public void
        Sulfuras_do_not_decrease_in_sell_in()
        {
            var stock = CreateSingleStockItemWith(name: EndOfDay.SULFURAS, sellIn: 1, quality: 1);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            var actual = endOfDay.Stock[0];
            Assert.Equal(1, actual.SellIn);
        }

        [Fact]
        public void
        Backstage_passes_increase_in_quality_by_1_over_10_days()
        {
            var stock = CreateSingleStockItemWith(name: EndOfDay.BACKSTAGE_PASSES, sellIn: 11, quality: 0);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            var actual = endOfDay.Stock[0];
            Assert.Equal(1, actual.Quality);
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
            var stock = CreateSingleStockItemWith(name: EndOfDay.BACKSTAGE_PASSES, sellIn: days, quality: 0);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            var actual = endOfDay.Stock[0];
            Assert.Equal(2, actual.Quality);
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
            IList<Item> stock = CreateSingleStockItemWith(name: EndOfDay.BACKSTAGE_PASSES, sellIn: days, quality: 0);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();
            var actual = endOfDay.Stock[0];

            Assert.Equal(3, actual.Quality);
        }

        [Fact]
        public void
        Backstage_passes_quality_is_zero_after_concert()
        {
            var stock = CreateSingleStockItemWith(name: EndOfDay.BACKSTAGE_PASSES, sellIn: 0, quality: 3);
            var endOfDay = new EndOfDay(stock);
            endOfDay.UpdateQuality();

            var actual = endOfDay.Stock[0];
            Assert.Equal(0, actual.Quality);
        }

        private static IList<Item> CreateSingleStockItemWith(string name, int sellIn, int quality)
        {
            return new List<Item>
            {
                new Item { Name = name, SellIn = sellIn, Quality = quality }
            };
        }
    }
}