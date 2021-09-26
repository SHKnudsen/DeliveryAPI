using System.Linq;
using DeliveryAPI.UserService.Utils;
using NUnit.Framework;

namespace UserServiceTests
{
    public class BogusDataTests
    {
        [Test]
        public void CanCreateBogusRoot()
        {
            // Arrange
            // Act
            var root = BogusData.GenerateBogusRoot();

            // Assert
            Assert.IsNotNull(root);
            Assert.IsNotNull(root.AccessWindow);
            Assert.IsNotNull(root.Order);
            Assert.IsNotNull(root.Recipient);
            Assert.IsNotNull(root.State);
        }

        [Test]
        public void CanCreateMultipleBogusRoots()
        {
            // Arrange
            var rootsToCreate = 5;

            // Act
            var roots = BogusData.GenerateBogusRoot(rootsToCreate);

            // Assert
            Assert.That(roots.Select(f => f.Order).Distinct().Count().Equals(rootsToCreate));
            Assert.That(roots.Select(f => f.Recipient).Distinct().Count().Equals(rootsToCreate));
            
            // As all these roots are created at the same time
            // the access window is the same for all.
            // Same goes for the State, they will all be 'Created'
            Assert.That(roots.Select(f => f.AccessWindow).Count().Equals(rootsToCreate));
            Assert.That(roots.Select(f => f.State).Count().Equals(rootsToCreate));
        }

        [Test]
        public void CanCreateBogusRecipent()
        {
            // Arrange
            // Act
            var recipent = BogusData.GenerateBogusRecipent();

            // Assert
            Assert.IsNotNull(recipent);
            Assert.IsNotNull(recipent.Address);
            Assert.IsNotNull(recipent.Email);
            Assert.IsNotNull(recipent.Name);
            Assert.IsNotNull(recipent.PhoneNumber);
        }


        [Test]
        public void CanCreateMultipleBogusRecipents()
        {
            // Arrange
            var recipentsToCreate = 5;

            // Act
            var recipents = BogusData.GenerateBogusRecipent(recipentsToCreate);

            // Assert
            Assert.That(recipents.Select(f => f.Address).Distinct().Count().Equals(recipentsToCreate));
            Assert.That(recipents.Select(f => f.Email).Distinct().Count().Equals(recipentsToCreate));
            Assert.That(recipents.Select(f => f.Name).Distinct().Count().Equals(recipentsToCreate));
            Assert.That(recipents.Select(f => f.PhoneNumber).Distinct().Count().Equals(recipentsToCreate));
        }


        [Test]
        public void CanCreateBogusOrder()
        {
            // Arrange
            // Act
            var order = BogusData.GenerateBogusOrder();

            // Assert
            Assert.IsNotNull(order);
            Assert.IsNotNull(order.OrderNumber);
            Assert.IsNotNull(order.Sender);
        }

        [Test]
        public void CanCreateMultipleBogusOrders()
        {
            // Arrange
            var ordersToCreate = 5;

            // Act
            var orders = BogusData.GenerateBogusOrder(ordersToCreate);

            // Assert
            Assert.That(orders.Select(f => f.OrderNumber).Distinct().Count().Equals(ordersToCreate));
            Assert.That(orders.Select(f => f.Sender).Distinct().Count().Equals(ordersToCreate));
        }
    }
}