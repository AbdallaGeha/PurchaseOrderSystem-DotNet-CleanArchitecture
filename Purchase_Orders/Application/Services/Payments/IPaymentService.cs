using Purchase_Orders.Application.Dtos.Payments;

namespace Purchase_Orders.Application.Services.Payments
{
    /// <summary>
    /// Defines business operations related to payments against
    /// purchase order statements.
    /// </summary>
    public interface IPaymentService
    {
        /// <summary>
        /// Check if we can insert a payment for the statement
        /// </summary>
        Task<bool> CanInsertPayment(Guid statementId);

        /// <summary>
        /// Get the data required to insert a payment
        /// for the specified statement considering previous statements and payments
        /// </summary>
        Task<InsertPaymentGetDetailsDto> GetInsertPaymentDetails(Guid statementId);

        /// <summary>
        /// Insert a new payment, considering previous accumulative statements amount
        /// and previous payments
        /// </summary>
        Task InsertPayment(RegularPaymentCreationDto paymentDto);
    }
}