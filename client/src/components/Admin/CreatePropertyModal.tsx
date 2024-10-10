import { useState } from 'react';
import { toast } from "react-hot-toast";
import { useErrorHandler } from '../../hooks/useErrorHandler';
import { api } from "../../http";
import Modal from "../Modal";

interface CreatePropertyModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export default function CreatePropertyModal({ isOpen, onClose }: CreatePropertyModalProps) {
    const { handleError } = useErrorHandler();
    const [propertyName, setPropertyName] = useState('');

    const handleCreateProperty = () => {
        if (!propertyName.trim()) {
            toast.error("Property name cannot be empty");
            return;
        }

        const newProperty = {
            name: propertyName
        };

        api.paper.createPaperProperty(newProperty)
            .then(() => {
                toast.success('Property created successfully');
                onClose();
            })
            .catch(handleError);
    };

    const handleKeyDown = (event: React.KeyboardEvent<HTMLDivElement>) => {
        if (event.key === 'Enter') {
            handleCreateProperty();
        }
    };

    if (!isOpen) return null;

    return (
        <Modal title="Create New Property" onClose={onClose}>
            <div className="flex flex-col space-y-4" onKeyDown={handleKeyDown}>
                <input
                    type="text"
                    name="propertyName"
                    placeholder="Property Name"
                    className="input input-bordered w-full"
                    value={propertyName}
                    onChange={(e) => setPropertyName(e.target.value)}
                />

                <button className="btn btn-primary" onClick={handleCreateProperty}>
                    Save Property
                </button>
            </div>
        </Modal>
    );
}