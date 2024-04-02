using DSG.POS.Common.Enumerations;
using Microsoft.Extensions.Options;
using Moq;
using Moq.Protected;
using Newtonsoft.Json;
using PointOfSale.Models;
using PointOfSale.Options;
using PointOfSale.Services;
using PosTransactionManager.IntegrationTests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Request = DSG.POS.PosTransactionManager.Models.Request;
using Response = DSG.POS.PosTransactionManager.Models.Response;



namespace PointOfSale.UnitTests
{
    public class TransactionManagerClientShould
    {
        [Fact]
        public async void RemoveItemFromTransactionWithDeleteItemCall()
        {
            Response.Transaction expectedTransactionResult = new Response.Transaction()
            {
                Header = new Response.Header()
                {
                    StoreNumber = 1,
                    RegisterNumber = 2,
                    TransactionNumber = 3,
                    StartDateTime = DateTime.UtcNow,
                },
                Items = new List<Response.Item>()
                {
                    new Response.Item()
                    {
                        Description = "Test Item 1",
                        UnitPrice = 0,
                        Quantity = 1,
                        Upc = "0123456789"
                    }
                }
            };

            // NOTE: transaction manager already verifying deletion call
            // verify here that we, on a successful request, get back a transaction object
            // and verify local transaction item object set
            TransactionManagerClient txnMgrClient = GetTransactionManagerClient(expectedTransactionResult);
            Response.Transaction transaction = await txnMgrClient.DeleteItem(0, 0, 0);

            Assert.NotNull(transaction);
            Assert.Equal(1, transaction.Header.StoreNumber);
            Assert.Equal(2, transaction.Header.RegisterNumber);
            Assert.Equal(3, transaction.Header.TransactionNumber);

            Assert.Single(transaction.Items);
        }

        [Fact]
        public async void ReturnANewTransactionObjectOnValidNewTransactionCall()
        {
            Response.Transaction expectedTransactionResult = new Response.Transaction()
            {
                Header = new Response.Header()
                {
                    StoreNumber = 1,
                    RegisterNumber = 2,
                    TransactionNumber = 1,
                    StartDateTime = DateTime.UtcNow,
                },
                Items = new List<Response.Item>()
            };

            TransactionManagerClient transactionManagerClient = GetTransactionManagerClient(expectedTransactionResult);
            Response.Transaction transaction = await transactionManagerClient.NewTransaction(1, 2);

            Assert.NotNull(transaction);
            Assert.Equal(1, transaction.Header.StoreNumber);
            Assert.Equal(2, transaction.Header.RegisterNumber);
            Assert.Equal(1, transaction.Header.TransactionNumber);
            Assert.Empty(transaction.Items);
            AssertDateTime.Equal(DateTime.UtcNow, transaction.Header.StartDateTime, TimeSpan.FromMinutes(1));
            Assert.Equal(DateTime.MinValue, transaction.Header.EndDateTime);
        }

        [Fact]
        public async void ReturnTheNewItemWithTransactionOnValidNewItemCall()
        {
            ReceiptItem receiptItem = new ReceiptItem()
            {
                Description = "Test Item 1",
                Price = 0,
                Quantity = 1,
                UPC = "0123456789"
            };

            Response.Transaction expectedTransactionResult = new Response.Transaction()
            {
                Header = new Response.Header()
                {
                    StoreNumber = 1,
                    RegisterNumber = 2,
                    TransactionNumber = 3,
                    StartDateTime = DateTime.UtcNow,
                    EndDateTime = DateTime.UtcNow.AddMinutes(3)
                },
                Items = new List<Response.Item>()
                {
                    new Response.Item()
                    {
                        Description = "Test Item 1",
                        UnitPrice = 0,
                        Quantity = 1,
                        Upc = "0123456789"
                    }
                }
            };

            TransactionManagerClient txnMgrClient = GetTransactionManagerClient(expectedTransactionResult);
            Response.Transaction transaction = await txnMgrClient.NewItem(1, 2, receiptItem);

            Assert.NotNull(transaction);
            Assert.Equal(1, transaction.Header.StoreNumber);
            Assert.Equal(2, transaction.Header.RegisterNumber);
            Assert.Equal(3, transaction.Header.TransactionNumber);
            Assert.Single(transaction.Items);

            Response.Item returnedItem = transaction.Items.First();

            Assert.Equal(expectedTransactionResult.Items[0].Description, returnedItem.Description);
            Assert.Equal(expectedTransactionResult.Items[0].Quantity, returnedItem.Quantity);
            Assert.Equal(expectedTransactionResult.Items[0].UnitPrice, returnedItem.UnitPrice);
            Assert.Equal(expectedTransactionResult.Items[0].Upc, returnedItem.Upc);
        }

        [Fact]
        public async Task ReturnUpdatedTransactionWhenSuccessfulTenderAdd()
        {
            Request.Tender expectedTender = new Request.Tender()
            {
                Amount = 19.99M,
                TenderType = TenderType.Cash
            };

            Response.Transaction expectedTransactionResult = new Response.Transaction()
            {
                Header = new Response.Header()
                {
                    StoreNumber = 1,
                    RegisterNumber = 2,
                    TransactionNumber = 3,
                    StartDateTime = DateTime.UtcNow,
                    EndDateTime = DateTime.UtcNow.AddMinutes(3)
                },
                Items = new List<Response.Item>()
                {
                    new Response.Item()
                    {
                        Description = "Test Item 1",
                        UnitPrice = 0,
                        Quantity = 1,
                        Upc = "0123456789"
                    }
                },
                Tenders = new List<Response.Tender>()
                {
                    new Response.Tender()
                    {
                        Amount = expectedTender.Amount,
                        TenderType = expectedTender.TenderType
                    }
                }
            };

            TransactionManagerClient transactionManagerClient = GetTransactionManagerClient(expectedTransactionResult);
            Response.Transaction transaction = await transactionManagerClient.NewTender(expectedTransactionResult.Header.StoreNumber, expectedTransactionResult.Header.RegisterNumber, expectedTender);

            Assert.NotNull(transaction);
            Assert.NotNull(transaction.Tenders);

            Response.Tender actualTender = transaction.Tenders[0];
            Assert.Equal(expectedTender.Amount, actualTender.Amount);
            Assert.Equal(expectedTender.TenderType, actualTender.TenderType);
        }



        private TransactionManagerClient GetTransactionManagerClient<T>(T testContentObject)
        {
            IOptions<TransactionManagerOptions> transactionManagerOptions = Microsoft.Extensions.Options.Options.Create(
                new TransactionManagerOptions()
                {
                    BaseUrl = "http://localhost",

                    TransactionOptions = new TransactionOptions()
                    {
                        Endpoints = new TransactionOptions.TransactionEndpoints()
                        {
                            NewTransaction = string.Empty,
                            ActiveTransaction = string.Empty,
                            FinalizeTransaction = string.Empty
                        }
                    },
                    ItemOptions = new ItemOptions()
                    {
                        Endpoints = new ItemOptions.ItemEndpoints()
                        {
                            NewItem = string.Empty,
                            DeleteItem = string.Empty
                        }
                    },
                    TenderOptions = new TenderOptions()
                    {
                        Endpoints = new TenderOptions.TenderEndpoints()
                        {
                            NewTender = string.Empty
                        }
                    }
                });

            Mock<HttpMessageHandler> mockHttpMessageHandler = new Mock<HttpMessageHandler>();

            mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync", ItExpr.IsAny<HttpRequestMessage>(), ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage()
                {
                    StatusCode = System.Net.HttpStatusCode.OK,
                    Content = new StringContent(JsonConvert.SerializeObject(testContentObject))
                });

            HttpClient httpClient = new HttpClient(mockHttpMessageHandler.Object);
            TransactionManagerClient transactionClient = new TransactionManagerClient(httpClient, transactionManagerOptions);

            return transactionClient;
        }
    }
}
