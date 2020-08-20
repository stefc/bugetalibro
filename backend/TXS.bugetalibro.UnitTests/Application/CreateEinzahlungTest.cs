﻿using System;
using System.Linq;
using System.Threading.Tasks;
using TXS.bugetalibro.Application.Contracts.Data;
using TXS.bugetalibro.Application.UseCases;
using TXS.bugetalibro.Domain.Entities;
using TXS.bugetalibro.UnitTests.Helper;
using Xunit;

namespace TXS.bugetalibro.UnitTests.Application
{
    public class CreateEinzahlungTest : ApplicationTest
    {
        public CreateEinzahlungTest(ApplicationFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public async Task TestInitial()
        {
            // (A)range 
            var request = new CreateEinzahlung.Request() {
                Datum = new DateTime(2010, 1, 1),
                Betrag = 100.50m
            };

            // (A)ction
            var response = await this.Mediator.Send(request);
            
            // (A)ssert
            Assert.Equal(100.50m, response);

            var dataStore = base.Get<IDataStore>();
            Assert.Equal(1, dataStore.Set<Einzahlung>().Count());
            var row = dataStore.Set<Einzahlung>().Single( record => record.Betrag == 100.50m);
            Assert.Equal(new DateTime(2010,1,1), row.Datum);
        }

        [Fact]
        public async Task TestNoDate()
        {
            // (A)range 
            var request = new CreateEinzahlung.Request() {
                Betrag = 100.50m
            };

            // (A)ction
            var response = await this.Mediator.Send(request);
            
            // (A)ssert
            Assert.Equal(100.50m, response);
             var dataStore = base.Get<IDataStore>();
            Assert.Equal(1, dataStore.Set<Einzahlung>().Count());
            var row = dataStore.Set<Einzahlung>().Single( record => record.Betrag == 100.50m);
            Assert.Equal(new DateTime(2019,12,24), row.Datum);
        }
    }
}
