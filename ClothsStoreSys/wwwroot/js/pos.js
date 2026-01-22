window.pos = window.pos || {};

window.pos.showInvoicePreview = async function (model) {
    if (!model) {
        return false;
    }

    const linesHtml = (model.lines || [])
        .map(line => `
            <tr>
                <td style="padding:4px 8px; border-bottom:1px solid #eee;">${line.name}</td>
                <td style="padding:4px 8px; text-align:center; border-bottom:1px solid #eee;">${line.qty}</td>
                <td style="padding:4px 8px; text-align:right; border-bottom:1px solid #eee;">${line.price}</td>
                <td style="padding:4px 8px; text-align:right; border-bottom:1px solid #eee;">${line.total}</td>
            </tr>
        `)
        .join('');

    const labels = model.labels || {};
    const html = `
        <div style="text-align:left;">
            <div style="margin-bottom:8px; font-size:14px;">
                <div>${labels.invoiceNumber || 'Invoice'}: <strong>${model.invoiceNumber}</strong></div>
                <div>${labels.date || 'Date'}: ${model.date}</div>
                <div>${labels.cashier || 'Cashier'}: ${model.cashier}</div>
                <div>${labels.paymentType || 'Payment'}: ${model.paymentType}</div>
            </div>
            <table style="width:100%; border-collapse:collapse; font-size:13px; margin-bottom:8px;">
                <thead>
                    <tr>
                        <th style="text-align:left; padding:4px 8px; border-bottom:1px solid #ddd;">${labels.item || 'Item'}</th>
                        <th style="text-align:center; padding:4px 8px; border-bottom:1px solid #ddd;">${labels.qty || 'Qty'}</th>
                        <th style="text-align:right; padding:4px 8px; border-bottom:1px solid #ddd;">${labels.price || 'Price'}</th>
                        <th style="text-align:right; padding:4px 8px; border-bottom:1px solid #ddd;">${labels.total || 'Total'}</th>
                    </tr>
                </thead>
                <tbody>
                    ${linesHtml}
                </tbody>
            </table>
            <div style="display:flex; justify-content:space-between; font-size:13px;">
                <span>${labels.subtotal || 'Subtotal'}</span>
                <span>${model.subtotal}</span>
            </div>
            <div style="display:flex; justify-content:space-between; font-size:13px;">
                <span>${labels.discount || 'Discount'}</span>
                <span>${model.discount}</span>
            </div>
            <div style="display:flex; justify-content:space-between; font-weight:600; font-size:14px;">
                <span>${labels.total || 'Total'}</span>
                <span>${model.total}</span>
            </div>
            <div style="display:flex; justify-content:space-between; font-size:13px; margin-top:4px;">
                <span>${labels.paid || 'Paid'}</span>
                <span>${model.paid}</span>
            </div>
            <div style="display:flex; justify-content:space-between; font-size:13px;">
                <span>${labels.remaining || 'Remaining'}</span>
                <span>${model.remaining}</span>
            </div>
        </div>
    `;

    if (!window.Swal) {
        return window.confirm(model.confirmText || 'Confirm');
    }

    const result = await window.Swal.fire({
        title: model.title || 'Invoice Preview',
        html,
        showCancelButton: true,
        confirmButtonText: model.confirmText || 'Confirm',
        cancelButtonText: model.cancelText || 'Cancel',
        width: 600
    });

    return result.isConfirmed === true;
};

window.pos.printInvoice = function () {
    window.print();
};
