import classNames from 'classnames';
import Link from 'next/link';
import { useRouter } from 'next/router';

import { Props } from './interfaces';

export const TabbedNav: (props: Props) => JSX.Element = ({
    content,
    navMenuList
}) => {

    const router = useRouter();
    const currentPathName: string = router?.asPath;

    const { ariaLabelText } = content;

    const generatedClasses: any = {
        wrapper: classNames('c-tabbed-nav'),
        item: classNames('c-tabbed-nav_item')
    }; 

    return (

        <nav className="c-tabbed-nav_nav" aria-label={ariaLabelText}>
            <ul role="menu" className="u-list-plain c-tabbed-nav_list">
                {navMenuList.map(({ url, text }, index) => {

                    const isActive: boolean = url === currentPathName;
                    
                    generatedClasses.link = classNames('c-tabbed-nav_link', {
                        ['c-tabbed-nav_link--active']: isActive
                    }); 

                    return (

                        <li key={index} role="none" className={generatedClasses.item}>
                            <Link href={url}>
                                <a role="menuitem" aria-current={isActive} className={generatedClasses.link}>
                                    <span>{text}</span>
                                </a>
                            </Link>
                        </li>

                    )

                })}
            </ul>
        </nav>

    )

}
