import { Props } from './interfaces';

export const SkipLinks: (props: Props) => JSX.Element = ({
    linkList
}) => {

    return (

        <div className="c-skip-links">
            {linkList.map(({ id, text }, index) => {

                return (

                    <p key={index} className="c-skip-links_item">
                        <a href={id} className="c-skip-links_link">{text}</a>
                    </p>

                )

            })}
        </div>

    )

}