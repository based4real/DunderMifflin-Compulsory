import { useNavigate } from "react-router-dom";
import Profile from "./Profile";
import ShopCart from "./ShopCart";
import ThemeSwitcher from "./ThemeSwitcher";

export default function Navigation() {
  const navigate = useNavigate();
    return (
        <div className="navbar">
          <div className="navbar-start">
            <div className="dropdown">
              <div tabIndex={0} role="button" className="btn btn-ghost lg:hidden">
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  className="h-5 w-5"
                  fill="none"
                  viewBox="0 0 24 24"
                  stroke="currentColor">
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth="2"
                    d="M4 6h16M4 12h8m-8 6h16" />
                </svg>
              </div>
              <ul
                tabIndex={0}
                className="menu menu-sm dropdown-content bg-base-100 rounded-box z-[1] mt-3 w-52 p-2 shadow">
                <li><a onClick={() => navigate("/shop")}>Shop</a></li>
              </ul>
            </div>
            <a className="btn btn-ghost text-xl" onClick={() => navigate("/")}>Dunder Mifflin</a>
          </div>
          <div className="navbar-center hidden lg:flex">
            <ul className="menu menu-horizontal px-1">
              <li><a onClick={() => navigate("/shop")}>Shop</a></li>
            </ul>
          </div>
          <div className="navbar-end gap-2">
            <ShopCart/>
            <div className="form-control">
              <input type="text" placeholder="Search" className="input input-bordered w-24 md:w-auto" />
            </div>
            <Profile />
            <ThemeSwitcher />
          </div>
        </div>
    );
}