import { Props } from './interfaces'

/**
 * Derived from the NHS Design System Skip Link component: https://service-manual.nhs.uk/design-system/components/skip-link.
 * Used to help keyboard-only users skip to the main content on a page.
 */
export const SkipLinks: (props: Props) => JSX.Element = ({ linkList }) => {
    return (
        <div className="c-skip-links">
            {linkList.map(({ id, text, className }, index) => {
                return (
                    <p key={index} className={`c-skip-links_item ${className}`}>
                        <a href={id} className="c-skip-links_link">
                            {text}
                        </a>
                    </p>
                )
            })}
        </div>
    )
}
