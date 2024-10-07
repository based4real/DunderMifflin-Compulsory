import { useAtom } from "jotai";
import { ShopSearchAtom } from "../../atoms/ShopAtoms";
import { useNavigate, useLocation } from "react-router-dom";
import ClearableSearch from "../Input/ClearableSearch"; 

export default function ShopSearch() {
    const [searchTerm, setSearchTerm] = useAtom(ShopSearchAtom);
    const navigate = useNavigate();
    const location = useLocation();

    const handleSearchChange = (value: string) => {
        setSearchTerm(value);
        if (location.pathname !== "/shop") {
            navigate("/shop");
        }
    };

    const handleClearSearch = () => {
        setSearchTerm("");
    };

    return (
        <ClearableSearch
            searchTerm={searchTerm}
            onSearchChange={handleSearchChange}
            onClearSearch={handleClearSearch}
            promptText="Search for a product.."
        />
    );
}