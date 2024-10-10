import { useState, useEffect } from 'react';
import { OrderStatus } from "../../Api";
import { toast } from "react-hot-toast";
import { api } from "../../http";
import Modal from "../Modal";

interface UpdateOrderStatusModalProps {
    isOpen: boolean;
    onClose: () => void;
    orderId: number;
    currentStatus: OrderStatus;
}

export default function UpdateOrderStatusModal({isOpen, onClose, orderId, currentStatus}: UpdateOrderStatusModalProps) {
    const mapStatusToEnum = (status: string): OrderStatus => {
        switch (status.toLowerCase()) {
            case 'pending':
                return OrderStatus.Pending;
            case 'shipped':
                return OrderStatus.Shipped;
            case 'delivered':
                return OrderStatus.Delivered;
            default:
                throw new Error(`Unknown status: ${status}`);
        }
    };

    const [newStatus, setNewStatus] = useState<OrderStatus>(mapStatusToEnum(currentStatus));

    useEffect(() => {
        if (isOpen) {
            setNewStatus(mapStatusToEnum(currentStatus));
        }
    }, [isOpen, currentStatus]);
    
    const handleUpdateOrderStatus = () => {
        if (newStatus) {
            api.order
                .updateOrderStatus(orderId, { status: newStatus })
                .then(() => {
                    toast.success(`Order status updated to ${newStatus}`);
                    onClose();
                })
                .catch(() => {
                    toast.error("Failed to update order status.");
                });
        }
    };

    return (
        <Modal title="Update Order Status" onClose={onClose}>
            <div className="flex flex-col space-y-4">
                <div className="flex items-center justify-between gap-4">
                    <span className="font-medium">Order Status</span>
                    <select
                        className="select select-bordered w-full max-w-xs"
                        value={newStatus}
                        onChange={(e) => setNewStatus(e.target.value as OrderStatus)}
                    >
                        {Object.values(OrderStatus).map((status) => (
                            <option key={status} value={status}>
                                {status.charAt(0).toUpperCase() + status.slice(1)}
                            </option>
                        ))}
                    </select>
                </div>

                <button className="btn btn-primary" onClick={handleUpdateOrderStatus}>
                    Update Status
                </button>
            </div>
        </Modal>
    );
}