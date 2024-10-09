import { useState } from 'react';
import { FaWarehouse, FaDollarSign } from "react-icons/fa";
import { PaperCreateModel } from "../../Api";
import { toast } from "react-hot-toast";
import { useErrorHandler } from '../../hooks/useErrorHandler';
import { api } from "../../http";
import Modal from "../Modal";
import QuantityInput from "../Input/QuantityInput";
import ClearableSearch from '../Input/ClearableSearch';
import { useFetchProperties } from '../../hooks/useFetchProperties';

interface CreateProductModalProps {
    isOpen: boolean;
    onClose: () => void;
}

export default function CreateProductModal({ isOpen, onClose }: CreateProductModalProps) {
    const { handleError } = useErrorHandler();
    const [paperData, setPaperData] = useState<PaperCreateModel>({
        name: '',
        stock: 1,
        price: 1.00,
        propertyIds: []
    });
    const [selectedProperties, setSelectedProperties] = useState<number[]>([]);
    const [searchTerm, setSearchTerm] = useState("");

    const properties = useFetchProperties();
    
    const handleCreatePaper = () => {
        const newPaper: PaperCreateModel = {
            ...paperData,
            propertyIds: selectedProperties
        };

        api.paper.createPapers([newPaper])
            .then(() => {
                toast.success('Paper created successfully');
                onClose();
            })
            .catch(handleError);
    };

    const handleInputChange = (name: keyof PaperCreateModel, value: number | string) => {
        setPaperData(prevData => ({
            ...prevData,
            [name]: value,
        }));
    };

    const handleCheckboxChange = (propertyId: number) => {
        setSelectedProperties(prevSelected =>
            prevSelected.includes(propertyId)
                ? prevSelected.filter(id => id !== propertyId)
                : [...prevSelected, propertyId]
        );
    };

    const handleKeyDown = (event: React.KeyboardEvent<HTMLDivElement>) => {
        if (event.key === 'Enter') {
            handleCreatePaper();
        }
    };

    const filteredProperties = properties.filter(property =>
        property.name.toLowerCase().includes(searchTerm.toLowerCase())
    );

    if (!isOpen) return null;

    return (
        <Modal title="Create New Product" onClose={onClose}>
            <div className="flex flex-col space-y-4" onKeyDown={handleKeyDown}>
                <input
                    type="text"
                    name="name"
                    placeholder="Product Name"
                    className="input input-bordered w-full"
                    value={paperData.name}
                    onChange={(e) => handleInputChange('name', e.target.value)}
                />

                <label className="input input-bordered flex items-center gap-2">
                    <FaWarehouse className="h-4 w-4 opacity-70" />
                    <QuantityInput
                        max={2147483647} // int32 max
                        value={paperData.stock}
                        onChange={(value) => handleInputChange('stock', value)}
                    />
                </label>

                <label className="input input-bordered flex items-center gap-2">
                    <FaDollarSign className="h-4 w-4 opacity-70" />
                    <QuantityInput
                        max={Number.MAX_VALUE} // double max
                        value={paperData.price}
                        onChange={(value) => handleInputChange('price', value)}
                    />
                </label>
                
                <div>
                    <div className="flex justify-between items-center mb-2">
                        <h4 className="font-bold">Properties</h4>

                        <div className="w-64">
                            <ClearableSearch
                                searchTerm={searchTerm}
                                onSearchChange={setSearchTerm}
                                onClearSearch={() => setSearchTerm('')}
                                promptText="Search properties..."
                            />
                        </div>
                    </div>
                    
                    <div className="max-h-32 overflow-y-auto flex flex-col gap-2">
                        {filteredProperties.length > 0 ? (
                            filteredProperties.map((property) => (
                                <label key={property.id} className="cursor-pointer flex items-center">
                                    <input
                                        type="checkbox"
                                        className="checkbox checkbox-primary"
                                        checked={selectedProperties.includes(property.id)}
                                        onChange={() => handleCheckboxChange(property.id)}
                                    />
                                    <span className="ml-2">{property.name}</span>
                                </label>
                            ))
                        ) : (
                            <p className="text-gray-500">No properties found.</p>
                        )}
                    </div>
                </div>
                
                <button className="btn btn-primary" onClick={handleCreatePaper}>
                    Save Product
                </button>
            </div>
        </Modal>
    );
}