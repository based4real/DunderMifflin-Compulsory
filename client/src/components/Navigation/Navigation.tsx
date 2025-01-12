import { useNavigate, useLocation } from "react-router-dom";
import Profile from "./Profile";
import ShopCart from "./ShopCart";
import ThemeSwitcher from "./ThemeSwitcher";
import ShopSearch from "./ShopSearch";

export default function Navigation() {
  const navigate = useNavigate();
  const location = useLocation(); 

  return (
    <div className="bg-base-100 text-base-content sticky top-0 z-30 flex h-16 w-full justify-center bg-opacity-90 backdrop-blur transition-shadow duration-100 shadow-sm">
    <div className="navbar px-6">
      <div className="flex w-full items-center justify-between">
        <div className="navbar-start flex items-center space-x-4">
          <a className="btn btn-ghost text-xl" onClick={() => navigate("/")}>
            Dunder Mifflin
          </a>
          <div className="border-l border-gray-300 h-6"></div>

          <a className={`btn btn-ghost ${ location.pathname === "/" ? "btn-active" : "" }`} onClick={() => navigate("/")}>
            Home
          </a>

          <a className={`btn btn-ghost ${ location.pathname === "/shop" ? "btn-active" : ""}`} onClick={() => navigate("/shop")}>
            Shop
          </a>
        </div>

        <div className="navbar-center w-96 flex-grow">
          <ShopSearch />
        </div>

        <div className="navbar-end flex items-center space-x-4">
          <ShopCart />
          <Profile />
          <ThemeSwitcher />
        </div>
      </div>
    </div>
    </div>
  );
}
