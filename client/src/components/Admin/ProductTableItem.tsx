import { FaWarehouse, FaBan } from "react-icons/fa";
import { PaperDetailViewModel } from "../../Api";

interface ProductTableItemProps {
    paper: PaperDetailViewModel;
    selected: boolean;
    onDiscontinue: (paperId: number) => void;
    onToggleSelect: (isSelected: boolean) => void;
    onRestock: (paperId: number, paperName: string) => void;
}

export default function ProductTableItem({ paper, selected, onDiscontinue, onToggleSelect, onRestock }: ProductTableItemProps) {
    const properties = paper.properties ?? [];

    const handleCheckboxChange = (e: React.ChangeEvent<HTMLInputElement>) => {
        onToggleSelect(e.target.checked);
    };

    return (
        <tr>
            <th>
                <label>
                    <input
                        type="checkbox"
                        className="checkbox"
                        checked={selected}
                        onChange={handleCheckboxChange}
                    />
                </label>
            </th>
            <td>
                <div>
                    <div className="font-bold">{paper.name}</div>
                    <div className={`text-sm opacity-50 ${paper.discontinued ? "text-error" : "text-success"}`}>
                        {paper.discontinued ? "Discontinued" : "Available"}
                    </div>
                </div>
            </td>

            <td>
                {properties.length > 0 ? (
                    <>
                        {properties[0].name}
                        {properties.length > 1 && (
                            <>
                                <br />
                                <div className="tooltip" data-tip={properties.slice(1).map(p => p.name).join(", ")}>
                                    <span className="badge badge-neutral cursor-pointer badge-sm">
                                        +{properties.length - 1} others
                                    </span>
                                </div>
                            </>
                        )}
                    </>
                ) : (
                    <span>No properties</span>
                )}
            </td>

            <td>{paper.stock}</td>
            <td>${paper.price.toFixed(2)}</td>
            <th className="text-right">
                <div className="join join-vertical lg:join-horizontal">
                    <button className="btn text-accent join-item" onClick={() => onRestock(paper.id, paper.name)} disabled={paper.discontinued}>
                        <FaWarehouse />
                    </button>
                    <button className="btn text-error join-item" onClick={() => onDiscontinue(paper.id)} disabled={paper.discontinued}>
                        <FaBan />
                    </button>
                </div>
            </th>
        </tr>
    );
}