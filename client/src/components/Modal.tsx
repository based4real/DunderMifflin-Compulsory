import React, { useEffect, useRef } from "react";

interface ModalProps {
    title: string;
    onClose: () => void;
    children: React.ReactNode;
}

export default function Modal({ title, onClose, children } : ModalProps) {
    const modalRef = useRef<HTMLDialogElement>(null);

    useEffect(() => {
        if (!modalRef.current) return;
        
        const dialogElement = modalRef.current;
        dialogElement.showModal();

        const handleCancel = () => {
            dialogElement.close();
            onClose();
        };

        dialogElement.addEventListener('cancel', handleCancel);

        return () => dialogElement.removeEventListener('cancel', handleCancel);
    }, [onClose]);

    const closeModal = () => {
        if (!modalRef.current) return;
        
        modalRef.current.close();
        onClose();
    };

    return (
        <dialog ref={modalRef} className="modal modal-bottom sm:modal-middle">
            <div className="modal-box relative">
                {/* Corner close button */}
                <button className="btn btn-sm btn-circle btn-ghost absolute right-2 top-2" onClick={closeModal}>
                    ✕
                </button>

                <h3 className="font-bold text-lg">{title}</h3>
                <div className="py-4">{children}</div>
            </div>
        </dialog>
    );
};
