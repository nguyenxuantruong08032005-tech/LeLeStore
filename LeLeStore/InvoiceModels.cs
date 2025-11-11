using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LeLeStore
{
    public class InvoiceLine
    {
        public int Sequence { get; set; }

        public int ProductId { get; set; }

        public string ProductName { get; set; } = string.Empty;

        public int Quantity { get; set; }

        public decimal UnitPrice { get; set; }

        public decimal Total => UnitPrice * Quantity;

        public InvoiceLine Clone()
        {
            return new InvoiceLine
            {
                Sequence = Sequence,
                ProductId = ProductId,
                ProductName = ProductName,
                Quantity = Quantity,
                UnitPrice = UnitPrice
            };
        }
    }

    public class InvoiceSnapshot
    {
        public InvoiceSnapshot(IEnumerable<InvoiceLine> lines, DateTime createdAt, int? employeeId, string employeeUsername)
        {
            if (lines == null)
            {
                throw new ArgumentNullException(nameof(lines));
            }

            var materialized = new List<InvoiceLine>();

            foreach (var line in lines)
            {
                if (line == null)
                {
                    continue;
                }

                materialized.Add(line.Clone());
            }

            Lines = new ReadOnlyCollection<InvoiceLine>(materialized);
            CreatedAt = createdAt;
            EmployeeId = employeeId;
            EmployeeUsername = employeeUsername ?? string.Empty;
        }

        public IReadOnlyList<InvoiceLine> Lines { get; }

        public DateTime CreatedAt { get; }

        public int? EmployeeId { get; }

        public string EmployeeUsername { get; }

        public decimal TotalAmount => Lines.Sum(line => line?.Total ?? 0m);
    }
}
