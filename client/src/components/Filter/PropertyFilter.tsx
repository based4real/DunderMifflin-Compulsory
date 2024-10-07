import { useEffect } from "react";
import { api } from "../../http";
import { PaperPropertiesSummaryAtom } from "../../atoms/PaperPropertiesSummaryAtom";
import {useAtom} from "jotai/index";

interface PropertyFilterProps {
    selectedProperties: number[];
    onPropertiesChange: (selected: number[]) => void;
}

export default function PropertyFilter({ selectedProperties, onPropertiesChange }: PropertyFilterProps) {
    const [properties, setProperties] = useAtom(PaperPropertiesSummaryAtom);

    useEffect(() => {
        api.paper.allProperties()
            .then(response => {
                setProperties(response.data ?? []);
            })
            .catch(error => {
                console.error("Error fetching properties: ", error);
            });
    }, []);

    const handleCheckboxChange = (propertyId: number) => {
        if (selectedProperties.includes(propertyId)) {
            onPropertiesChange(selectedProperties.filter(id => id !== propertyId));
        } else {
            onPropertiesChange([...selectedProperties, propertyId]);
        }
    };

    return (
        <div className="form-control mb-4">
            <h3 className="font-semibold mb-2">Filter by Properties</h3>
            {properties.map((property) => (
                <label key={property.id} className="label cursor-pointer">
                    <input
                        type="checkbox"
                        className="checkbox checkbox-primary"
                        checked={selectedProperties.includes(property.id!)}
                        onChange={() => handleCheckboxChange(property.id!)}
                    />
                    <span className="label-text ml-2">{property.name}</span>
                </label>
            ))}
        </div>
    );
}