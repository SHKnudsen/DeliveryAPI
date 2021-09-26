using System.Collections.Generic;
using System.Linq;
using Bogus;
using DeliveryAPI.Core.Models;

namespace DeliveryAPI.UserService.Utils
{
    /// <summary>
    /// Utility class to create fake date for orders
    /// </summary>
    public class BogusData
    {
        /// <summary>
        /// Create a new Root using fake data
        /// </summary>
        /// <returns></returns>
        public static Root GenerateBogusRoot()
        {
            var root = new Root(
                GenerateBogusRecipent(),
                GenerateBogusOrder());

            return root;
        }

        /// <summary>
        /// Create a new Roots using fake data
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Root> GenerateBogusRoot(int amount)
        {
            var roots = Enumerable.Range(0, amount)
                .Select(i => new Root(
                    GenerateBogusRecipent(),
                    GenerateBogusOrder()));

            return roots;
        }

        /// <summary>
        /// Creates a new <see cref="Recipient"/> using fake data
        /// </summary>
        /// <returns></returns>
        public static Recipient GenerateBogusRecipent()
        {
            return CreateFakeRecipent(1).FirstOrDefault();
        }

        /// <summary>
        /// Creates new <see cref="Recipient"/>s using fake data
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Recipient> GenerateBogusRecipent(int amount)
        {
            return CreateFakeRecipent(amount);
        }


        /// <summary>
        /// Creates a new <see cref="Order"/> using fake data
        /// </summary>
        /// <returns></returns>
        public static Order GenerateBogusOrder()
        {
            return CreateFakeOrders(1).FirstOrDefault();
        }

        /// <summary>
        /// Creates new <see cref="Order"/>s using fake data
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Order> GenerateBogusOrder(int amount)
        {
            return CreateFakeOrders(amount);
        }

        #region Private helpers

        private static IEnumerable<Recipient> CreateFakeRecipent(int amount)
        {
            var faker = new Faker<Recipient>()
                .RuleFor(b => b.Address, f => f.Address.FullAddress())
                .RuleFor(b => b.Email, f => f.Person.Email)
                .RuleFor(b => b.Name, f => f.Name.FullName())
                .RuleFor(b => b.PhoneNumber, f => f.Phone.PhoneNumber());

            var fakes = faker.Generate(amount);
            return fakes;
        }

        private static IEnumerable<Order> CreateFakeOrders(int amount)
        {
            var faker = new Faker<Order>()
                .RuleFor(b => b.OrderNumber, f => f.Random.Guid().ToString())
                .RuleFor(b => b.Sender, f => f.Company.CompanyName())
                .RuleFor(b => b.SenderEmail, f => f.Person.Email);

            var fakes = faker.Generate(amount);
            return fakes;
        }

        #endregion
    }
}
