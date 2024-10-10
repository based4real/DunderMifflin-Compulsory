import { useState } from "react";

export function useProductSelection(papers: any[]) {
    const [selectedProductIds, setSelectedProductIds] = useState<number[]>([]);
    const [selectAll, setSelectAll] = useState(false);

    const handleSelectAll = (e: React.ChangeEvent<HTMLInputElement>) => {
        const isChecked = e.target.checked;
        setSelectAll(isChecked);
        setSelectedProductIds(isChecked ? papers.map(paper => paper.id) : []);
    };

    const handleToggleSelect = (isSelected: boolean, productId: number) => {
        setSelectedProductIds((prevSelectedIds) => {
            const updatedSelectedIds = isSelected
                ? [...prevSelectedIds, productId]
                : prevSelectedIds.filter((id) => id !== productId);

            setSelectAll(updatedSelectedIds.length === papers.length);

            return updatedSelectedIds;
        });
    };

    const handleDeselectAll = () => {
        setSelectedProductIds([]);
        setSelectAll(false);
    };

    const resetSelection = () => {
        setSelectedProductIds([]);
        setSelectAll(false);
    };

    return {
        selectedProductIds,
        selectAll,
        handleSelectAll,
        handleToggleSelect,
        handleDeselectAll,
        resetSelection
    };
}