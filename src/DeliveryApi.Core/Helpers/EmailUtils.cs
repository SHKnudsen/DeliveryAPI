using System;
using System.Collections.Generic;
using System.Text;
using DeliveryAPI.Core.Models;

namespace DeliveryApi.Core.Helpers
{
	/// <summary>
	/// Utility class to create emails.
	/// </summary>
	public class EmailUtils
	{
		/// <summary>
		/// Email body for when an order is created for a partner.
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public static string PartnerOrderRecivedEmailBody(Root root)
		{
			return $@"
				<html>
					{HeadStyle}
					<body>
						{Header}
						<p>Dear {root.Order.Sender},</p>
						
						<p>A new order has been created with the following information:</p>
						<table>
							<tr><th>Recipient name</th><td>{root.Recipient.Name}</td></tr>
							<tr><th>Phone</th><td>{root.Recipient.PhoneNumber}</td></tr>
							<tr><th>Email</th><td>{root.Recipient.Email}</td></tr>
							<tr><th>Address</th><td>{root.Recipient.Address}</td></tr>
						</table>
					</body>
				</html>
			";
		}

		/// <summary>
		/// Email body for when an order has been approved by a user.
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public static string PartnerOrderApprovedEmailBody(Root root)
		{
			return $@"
				<html>
					{HeadStyle}
					<body>
						{Header}
						<p>Dear {root.Order.Sender},</p>
						
						<p>Order {root.Order.OrderNumber} was recently approved by the user.</p>
						<p>The order will expire at <b>{root.AccessWindow.EndTime}</b>, please make sure to <code>Complete</code> or <code>Cancel</code> this order before it expires.</p>

						<p>Customer Information:</p>
						<table>
							<tr><th>Recipient name</th><td>{root.Recipient.Name}</td></tr>
							<tr><th>Phone</th><td>{root.Recipient.PhoneNumber}</td></tr>
							<tr><th>Email</th><td>{root.Recipient.Email}</td></tr>
							<tr><th>Address</th><td>{root.Recipient.Address}</td></tr>
						</table>
					</body>
				</html>
			";
		}

		/// <summary>
		/// Email body for when partner has completed an order.
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public static string PartnerOrderCompletedEmailBody(Root root)
		{
			return $@"
				<html>
					{HeadStyle}
					<body>
						{Header}
						<p>Dear {root.Recipient.Name},</p>
						
						<p>Your order {root.Order.OrderNumber} was recently completed by {root.Order.Sender}</p>
					</body>
				</html>
			";
		}

		/// <summary>
		/// Email body to partner when an active order has expired.
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public static string PartnerOrderExpiredEmailBody(Root root)
		{
			return $@"
				<html>
					{HeadStyle}
					<body>
						{Header}
						<p>Dear {root.Order.Sender},</p>
						
						<p>Active order {root.Order.OrderNumber} has now expired</p>
					</body>
				</html>
			";
		}

		/// <summary>
		/// Email body for partner when an order has been canceled.
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public static string PartnerOrderCanceledEmailBody(Root root)
		{
			return $@"
				<html>
					{HeadStyle}
					<body>
						{Header}
						<p>Dear {root.Order.Sender},</p>
						
						<p>Active order {root.Order.OrderNumber} has been canceled</p>
					</body>
				</html>
			";
		}

		/// <summary>
		/// Email body for user when an order has expired.
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public static string UserOrderExpiredEmailBody(Root root)
		{
			return $@"
				<html>
					{HeadStyle}
					<body>
						{Header}
						<p>Dear {root.Recipient.Name},</p>
						
						<p>Your order {root.Order.OrderNumber}, to {root.Order.Sender}, has now expired and our partner will no longer be able to complete the order</p>
					</body>
				</html>
			";
		}

		/// <summary>
		/// Email body for user when an order has been canceled.
		/// </summary>
		/// <param name="root"></param>
		/// <returns></returns>
		public static string UserOrderCanceledEmailBody(Root root)
		{
			return $@"
				<html>
					{HeadStyle}
					<body>
						{Header}
						<p>Dear {root.Recipient.Name},</p>
						
						<p>Your order {root.Order.OrderNumber}, to {root.Order.Sender}, has been canceled</p>
					</body>
				</html>
			";
		}

		#region Private Helpers

		private static string HeadStyle => @"
			<head>
				<style>
					body {
						font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
					}
					table { 
						text-align: left;
						padding-top: 10px;
					}
					th {
						width: 200px;
						padding-left: 10px;
						background-clip: content-box;
						background-color: #EEEEEE;
						font-weight: normal;
					}
					td {
						padding: 5px;
						width: 300px;
						border: 1px solid black;
					}
					.fine {
						font-weight: bold;
						color: #FF0000;
					}
					.logo {
						float: left;
						display: block;
						margin-top: -15px;
					}
					.title {
						display: block;
					}
					.logo-name {
						color: #FFFFFF;
						background-color: #2AA3D9;
						vertical-align: middle;
						padding: 10px;
						margin-top: 20px;
						height: 20px;
						width: 400px;
					}
					.logo-bar {
						background-color: #005D91;
						width: 420px;
						height: 20px;
						margin-top: -22px;
						margin-bottom: 30px;
					}
				</style>
			</head>
		";

		private static string Header => @"
			<div class='logo'>
				<svg version='1.1' width='105px' height='85px' viewBox='-0.5 -0.5 105 85'><defs/><g><path d='M 25.51 71.39 L 45.51 31.2 L 97.04 31.2 L 77.04 71.39 Z' fill='#000000' stroke='none' transform='translate(2,3)rotate(-15,61.27,51.29)' opacity='0.25'/><path d='M 25.51 71.39 L 45.51 31.2 L 97.04 31.2 L 77.04 71.39 Z' fill='#e6e6e6' stroke='none' transform='rotate(-15,61.27,51.29)' pointer-events='all'/><path d='M 15.51 60.24 L 35.51 20.05 L 87.04 20.05 L 67.04 60.24 Z' fill='#000000' stroke='none' transform='translate(2,3)rotate(-15,51.27,40.14)' opacity='0.25'/><path d='M 15.51 60.24 L 35.51 20.05 L 87.04 20.05 L 67.04 60.24 Z' fill='#2aa3d9' stroke='none' transform='rotate(-15,51.27,40.14)' pointer-events='all'/><path d='M 4.39 49.08 L 24.39 8.89 L 75.92 8.89 L 55.92 49.08 Z' fill='#000000' stroke='none' transform='translate(2,3)rotate(-15,40.16,28.99)' opacity='0.25'/><path d='M 4.39 49.08 L 24.39 8.89 L 75.92 8.89 L 55.92 49.08 Z' fill='#005d91' stroke='none' transform='rotate(-15,40.16,28.99)' pointer-events='all'/></g></svg>
			</div>
			<div class='title'>
				<h4 class='logo-name'>Partner Delivery API</h4>
				<div class='logo-bar'>&nbsp;</div>
			</div>
		";

		#endregion
	}
}
