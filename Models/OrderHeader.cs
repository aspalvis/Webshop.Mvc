﻿using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Models
{
    public class OrderHeader
    {
        [Key]
        public int Id { get; set; }

        public string CreatedByUserId { get; set; }

        [ForeignKey(nameof(CreatedByUserId))]
        public ApplicationUser CreatedBy { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? ShippingDate { get; set; }

        public double FinalOrderTotal { get; set; }

        public string OrderStatus { get; set; }

        public DateTime? PaymentDate { get; set; }

        public DateTime? PaymentDueDate { get; set; }

        public string TransactionId { get; set; }

        [Required]
        public string PhoneNumber { get; set; }

        [Required]
        public string StreetAddress { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string State { get; set; }

        [Required]
        public string PostalCode { get; set; }

        [Required]
        public string FullName { get; set; }

        public string Email { get; set; }
    }
}
