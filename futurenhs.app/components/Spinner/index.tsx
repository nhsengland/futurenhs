import classNames from 'classnames'

import { Props } from './interfaces'

/**
 * SVG based spinner with configurable speed.
 */
export const Spinner: (props: Props) => JSX.Element = ({
    speed = 'medium',
    className,
}) => {
    const generatedClasses: any = {
        wrapper: classNames(`c-spinner u-transform-gpu`, className, {
            ['u-animate-[spin_3.5s_linear_infinite]']: speed === 'slow',
            ['u-animate-[spin_2s_linear_infinite]']: speed === 'medium',
            ['u-animate-[spin_1.25s_linear_infinite]']: speed === 'fast',
        }),
    }

    return (
        <div className={generatedClasses.wrapper}>
            {[...Array(4)].map((_, i) => {
                const key: string = i.toString()
                const className: any = classNames('c-spinner_item')

                return (
                    <svg
                        key={key}
                        className={className}
                        xmlns="http://www.w3.org/2000/svg"
                        width="72"
                        height="72"
                        viewBox="0 0 19.05 19.050001"
                        version="1.1"
                    >
                        <g transform="translate(0,-277.94998)">
                            <path
                                d="M 15.958984 0 L 0 15.960938 L 0 56 L 0 72 L 56.044922 72 L 72 56.044922 L 72 56 L 16 56 L 16 0 L 15.958984 0 z "
                                transform="matrix(0.26458333,0,0,0.26458333,0,277.94998)"
                            />
                        </g>
                    </svg>
                )
            })}
        </div>
    )
}
